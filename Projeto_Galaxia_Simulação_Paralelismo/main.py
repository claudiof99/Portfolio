"""
main.py - Galaxy Simulation Entry Point

MPI orchestration:
  - All ranks generate their own particle slice in parallel
  - Rank 0 broadcasts params, scatters per-rank counts
  - All ranks compute forces (physics.py) and update particles
  - Every PLOT_EVERY simulated years rank 0 gathers data and saves a frame
  - Rank 0 builds the GIF at the end

MPI methods used:
  bcast()      — broadcast simulation parameters from rank 0 to all
  scatter()    — distribute per-rank particle counts from rank 0
  allgather()  — share masses globally; share positions in physics.py
  gather()     — collect positions on rank 0 for plotting
  allreduce()  — sum local kinetic energies into total KE
  reduce()     — collect max/min/avg wall-time across ranks
  Barrier()    — synchronise all ranks before/after timed section
  Isend/Irecv  — non-blocking ready pings from workers to rank 0
  Send/Recv    — blocking acknowledgement from rank 0 back to workers

Run with:
    mpirun -n 4 python main.py

    mpirun -n 4 python main.py --particles 1000 --steps 50 --dt-max 1000000 --plot-every 5000000
        (quick test: 50 steps, 50M yr simulated, ~10 frames, ~1-2 min)

    mpirun -n 4 python main.py --particles 7500 --steps 200 --dt-max 50000000 --plot-every 500000000
        (full run: 7500 particles, 500M yr simulated, ~50 frames, ~878s)
"""

import glob

import numpy as np
import os
import argparse
from mpi4py import MPI

from initial_conditions import generate_galaxy_parallel
from plotting import save_frame
from animation import build_gif

from constants import YEAR_LIGHT, YEAR_IN_SECONDS


# ---- MPI setup -----------------------------
comm = MPI.COMM_WORLD
rank = comm.Get_rank()
size = comm.Get_size()

# Physical constant


# -- CLI arguments (rank 0 parses, then broadcasts) ------------------------
def parse_args():
    parser = argparse.ArgumentParser(description="Parallel Galaxy Simulation")
    parser.add_argument("--particles",  type=int,   default=10000,
                        help="Total number of star particles")
    parser.add_argument("--steps",      type=int,   default=10000,
                        help="Total simulation time steps")
    parser.add_argument("--dt-max",         type=float, default=1e3,
                        help="Maximum time step size in years")
    parser.add_argument("--dt-min",         type=float, default=10.0,
                        help="Minimum time step size in years")
    parser.add_argument("--plot-every", type=float, default=1000,
                    help="Save a frame every N simulated years (default: 1000)")
    parser.add_argument("--frames-dir", type=str,   default="frames",
                        help="Directory to save plot frames")
    parser.add_argument("--output-gif", type=str,   default="output/galaxy.gif",
                        help="Path for the output GIF")
    
    args, _ = parser.parse_known_args()
    return args


def move(points, velocities, accelerations, dt):
    velocities += accelerations * dt
    points += velocities * dt


def main():
    # -- 1. Parse & broadcast simulation parameters --------------------
    if rank == 0:
        args   = parse_args()
        params = {
            "N":          args.particles,
            "STEPS":      args.steps,
            "DT_MAX":     args.dt_max,
            "DT_MIN":     args.dt_min,
            "PLOT_EVERY": args.plot_every,
            "FRAMES_DIR": args.frames_dir,
            "OUTPUT_GIF": args.output_gif
        }
        os.makedirs(params["FRAMES_DIR"], exist_ok=True)
        os.makedirs(os.path.dirname(params["OUTPUT_GIF"]) or ".", exist_ok=True)

        for old_frame in glob.glob(os.path.join(params["FRAMES_DIR"], "frame_*.png")):
            os.remove(old_frame)
        
        print(f"[rank 0] Starting simulation: N={params['N']}, "
              f"steps={params['STEPS']}, dt={params['DT_MAX']} yr, ranks={size}")
    else:
        params = None

    # bcast: rank 0 sends the params dict to every other rank
    params = comm.bcast(params, root=0)

    N          = params["N"]
    STEPS      = params["STEPS"]
    #  convert years -> seconds for physics
    DT_MAX     = params["DT_MAX"] * YEAR_IN_SECONDS
    DT_MIN     = params["DT_MIN"] * YEAR_IN_SECONDS
    PLOT_EVERY = params["PLOT_EVERY"]
    FRAMES_DIR = params["FRAMES_DIR"]
    OUTPUT_GIF = params["OUTPUT_GIF"]

    # -- 2. Parallel initial condition generation ------------------------
    # Each rank independently generates its own slice of particles.
    # This parallelises the IC generation (requirement: not just rank 0).
    local_pos, local_vel, local_masses = generate_galaxy_parallel(N, rank, size)

    # allgather: every rank shares its local_masses so every rank has the
    # full mass array needed for force computation.
    all_masses_chunks = comm.allgather(local_masses)
    masses = np.concatenate(all_masses_chunks)   # shape (N,)

    # scatter: rank 0 distributes per-rank particle counts to each rank.
    # This confirms each rank's local_N and demonstrates scatter usage.
    # rank 0 builds the list of counts; every rank receives its own integer.
    if rank == 0:
        count_list = [len(chunk) for chunk in all_masses_chunks]
    else:
        count_list = None
    my_count = comm.scatter(count_list, root=0)
    assert my_count == local_pos.shape[0], \
        f"[rank {rank}] scatter count mismatch: {my_count} vs {local_pos.shape[0]}"

    local_N = local_pos.shape[0]

    if rank == 0:
        print(f"[rank 0] Particles per rank: ~{local_N}  |  "
              f"DT = {DT_MAX / YEAR_IN_SECONDS} yr = {DT_MAX:.3e} s")

    # -- 3. Import physics (after MPI init) -------------------------------
    from physics import compute_forces_numpy as compute_forces

    # -- 4. Point-to-point status: workers notify rank 0 they are ready ----
    # isend/irecv — non-blocking: workers fire off their ready token
    #               without blocking, then immediately wait.
    # send/recv   — blocking: rank 0 sends an acknowledgement back to each
    #               worker so every rank knows it was registered.
    if rank != 0:
        # Non-blocking send: worker notifies rank 0 it is ready
        ready_msg = np.array([rank], dtype=np.int32)
        req = comm.Isend(ready_msg, dest=0, tag=10)
        req.Wait()
        # Blocking recv: wait for rank 0's acknowledgement
        ack = np.empty(1, dtype=np.int32)
        comm.Recv(ack, source=0, tag=11)
    else:
        for src in range(1, size):
            # Non-blocking recv: collect each worker's ready token
            buf = np.empty(1, dtype=np.int32)
            req = comm.Irecv(buf, source=src, tag=10)
            req.Wait()
            print(f"[rank 0] Rank {buf[0]} is ready.")
            # Blocking send: acknowledge back to that worker
            ack = np.array([0], dtype=np.int32)
            comm.Send(ack, dest=src, tag=11)
        
    # -- 5. Main time loop --
    # Barrier: every rank must reach here before timing starts
    comm.Barrier()
    t_start = MPI.Wtime()  # MPI's wall clock time for better accuracy in parallel runs

    energy_log = []
    sim_time_yr = 0.0     # accumulated simulated time in years
    last_plot_yr = -1.0   # ensures step 0 always plots

    for step in range(STEPS):
        
        local_acc = compute_forces(local_pos, masses, comm)
        local_max_acc = np.max(np.linalg.norm(local_acc, axis=1))
        global_max_acc = comm.allreduce(local_max_acc, op=MPI.MAX)

        if global_max_acc > 1e-5:
            dt = max(DT_MIN, min(DT_MAX, 0.1 * np.sqrt(YEAR_LIGHT / global_max_acc)))
        else:
            dt = DT_MAX

        move(local_pos, local_vel, local_acc, dt)
        
        local_KE_per_particle = 0.5 * local_masses * np.sum(local_vel**2, axis=1)
        sim_time_yr += dt / YEAR_IN_SECONDS

        # --- Plotting: every PLOT_EVERY simulated years ---
        if sim_time_yr - last_plot_yr >= PLOT_EVERY:
            last_plot_yr = sim_time_yr
            all_pos_chunks = comm.gather(local_pos, root=0)
            all_KE_chunks  = comm.gather(local_KE_per_particle, root=0)

            if rank == 0:
                all_positions = np.vstack(all_pos_chunks)
                all_KE = np.concatenate(all_KE_chunks)
                total_KE = np.sum(all_KE)
                save_frame(all_positions, step, sim_time_yr, all_KE, total_KE, FRAMES_DIR)
                elapsed = MPI.Wtime() - t_start
                print(f"  step {step:6d}/{STEPS}  |  "
                      f"sim_time = {sim_time_yr:,.0f} yr  |  "
                      f"KE = {total_KE:.3e} J  |  "
                      f"wall = {elapsed:.1f}s")
                energy_log.append((step, sim_time_yr, total_KE))

    #  6. Final sync & timing -----------------------------------
    comm.Barrier()
    t_end      = MPI.Wtime()
    local_time = t_end - t_start
    
    # reduce: collect the maximum wall-time across all ranks.
    # The slowest rank determines the true parallel runtime.
    max_time = comm.reduce(local_time, op=MPI.MAX, root=0)
    min_time = comm.reduce(local_time, op=MPI.MIN, root=0)
    avg_time = comm.reduce(local_time, op=MPI.SUM, root=0)

    if rank == 0:
        avg_time /= size
        print(f"\n{'='*55}")
        print(f"Simulation complete.")
        print(f"  Particles      : {N:,}")
        print(f"  Steps          : {STEPS:,}")
        print(f"  Simulated time : {STEPS * DT_MAX / YEAR_IN_SECONDS:,.0f} years")
        print(f"  MPI ranks      : {size}")
        print(f"  Wall time (max): {max_time:.2f}s  "
              f"min={min_time:.2f}s  avg={avg_time:.2f}s")
        print(f"  Time/step      : {max_time/STEPS*1000:.2f} ms")
        print(f"{'='*55}\n")

        # -- 7. Build GIF -----------------------------------------------------
        print(f"Building GIF from frames in '{FRAMES_DIR}'...")
        build_gif(FRAMES_DIR, OUTPUT_GIF)
        print(f"GIF saved to: {OUTPUT_GIF}")

        # -- 8. Save timing results  ---------------------------------
        with open("timing_results.txt", "a") as f:
            f.write(
                f"N={N}, steps={STEPS}, ranks={size}, "
                f"max={max_time:.2f}s, min={min_time:.2f}s, "
                f"avg={avg_time:.2f}s, "
                f"per_step={max_time/STEPS*1000:.2f}ms\n"
            )

        # -- 9. Save energy log --------------------------------------
        with open("energy_log.txt", "w") as f:
            f.write("step,sim_time_yr,total_KE_J\n")
            for s, t, e in energy_log:
                f.write(f"{s},{t:.0f},{e:.6e}\n")
        print("Energy log saved to: energy_log.txt")


if __name__ == "__main__":
    main()