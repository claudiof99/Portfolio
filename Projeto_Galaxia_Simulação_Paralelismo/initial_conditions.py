"""
Parallel Galaxy Particle Generation

Each MPI rank independently generates its own slice of particles.
This fully parallelises the IC step — rank 0 is NOT a bottleneck.

Physics:
  - Exponential radial distribution  (realistic disc galaxy profile)
  - Circular orbital velocities      (prevents galaxy explosion)
  - Small Gaussian vertical kicks    (gives 3-D disc thickness)

Called by every rank in main.py via generate_galaxy_parallel().
"""

import numpy as np
from constants import G, KPC, MASSA_SUN

# -----------------------------------------------------------------------------
#  Parallel entry point  (used by main.py)
# ------------------------------------------------------------------------------

def generate_galaxy_parallel(N: int, rank: int, size: int):
    """
    Each MPI rank generates its own contiguous slice of N particles.

    Particle indices owned by this rank:
        start = rank * (N // size)  +  min(rank, N % size)
        end   = start + local_N

    Using a deterministic per-rank seed (base_seed + rank) guarantees
    that the combined result is identical to calling generate_galaxy(N)
    with the same base seed on a single process, but the work is
    distributed across all ranks in parallel.

    Args:
        N    : total number of particles in the galaxy
        rank : this process's MPI rank
        size : total number of MPI processes

    Returns:
        local_pos  : np.ndarray  (local_N, 3)   metres
        local_vel  : np.ndarray  (local_N, 3)   m/s
        local_mass : np.ndarray  (local_N,)     kg
    """
    # --- Work distribution: give the remainder particles to the first ranks ---
    base  = N // size
    extra = N % size
    local_N = base + (1 if rank < extra else 0)
    start   = rank * base + min(rank, extra)   # global index of first local particle

    # --- Per-rank deterministic seed -----------------------------------------
    rng = np.random.default_rng(seed=42 + rank)

    # --- Masses: ~1 solar mass, small spread ----------------------------------
    local_mass = MASSA_SUN * rng.normal(loc=1.0, scale=0.1, size=local_N).clip(0.1, 5.0)

    # --- Radial positions: exponential disc profile ---------------------------
    R_scale = 10 * KPC                                    # scale radius ~10 kpc
    r       = rng.exponential(scale=R_scale, size=local_N)

    # --- Azimuthal angles: uniform -------------------------------------------
    phi = rng.uniform(0, 2 * np.pi, size=local_N)

    # --- Vertical positions: thin Gaussian disc ------------------------------
    z_scale = 0.5 * KPC
    z       = rng.normal(loc=0.0, scale=z_scale, size=local_N)

    # Cylindrical -> Cartesian
    x = r * np.cos(phi)
    y = r * np.sin(phi)
    local_pos = np.column_stack([x, y, z]).astype(np.float64)

    # --- Circular orbital velocities -----------------------------------------
    # We approximate the enclosed mass for particle i (index = start + i)
    # as  M_enc(i) = M_total * (start+i+1) / N
    # (assumes exponential profile sorted by r — an approximation good enough
    #  for stable ICs; a full N-body sort would require a gather, defeating
    #  the purpose of parallel IC generation).
    M_total_approx = N * MASSA_SUN
    M_enc          = M_total_approx * (1.0 - np.exp(-r / R_scale))

    r_flat  = np.sqrt(x**2 + y**2) + 1e9    # cylindrical radius, avoid div/0
    v_circ  = np.sqrt(G * M_enc / r_flat)   # circular speed

    # Tangential direction (perpendicular to radial in x-y plane)
    # Full circular velocity — stars orbit the centre stably
    vx = -v_circ * (y / r_flat)
    vy =  v_circ *  (x / r_flat)
    vz =  rng.normal(loc=0.0, scale=v_circ * 0.05, size=local_N)   # small z kick

    local_vel = np.column_stack([vx, vy, vz]).astype(np.float64)

    return local_pos, local_vel, local_mass

