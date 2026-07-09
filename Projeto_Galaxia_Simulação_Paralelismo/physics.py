"""
physics.py - Gravitational Force Computation

Each MPI rank calls compute_forces_numpy() with its local particles.

MPI used here:
  allgather() — every rank shares its local positions so every rank
                has the full position array needed for N-body forces,
                but each rank only computes accelerations for its own
                local_N particles  (work is divided, data is shared).

  compute_forces_numpy — memory-efficient loop, one particle at a time
"""

import numpy as np
from constants import G, SOFTENING

# Gravitational constant  [m^3 kg^-1 s^-2]

# Softening length: prevents force singularity when two particles are
# very close together (equivalent to giving each particle a finite radius).
# Value ~1 pc is physically motivated for galaxy simulations.

# -----------------------------------------------------------------------------
#  compute_forces_numpy — Memory-efficient force computation
#  Loop-based approach: processes one local particle at a time
#  Memory usage: ~180 KB per rank regardless of N (at N=7500).
#  Compute cost: O(N²), but no memory pressure or swapping.
#  For N > 10^6, consider Barnes-Hut tree or Particle-Mesh (FFT) methods.
# -----------------------------------------------------------------------------

def compute_forces_numpy(local_pos, masses, comm):
    """
    Memory-efficient gravitational acceleration.
    Computes forces one local particle at a time to avoid allocating
    the full (local_N x N x 3) interaction matrix in RAM.

   
    It uses ~180 KB per rank regardless of N.
    At N=10^6+ a Barnes-Hut tree or Particle-Mesh (FFT) method is needed
    to reduce the O(N²) compute cost itself (discussed in report).

    Args:
        local_pos : (local_N, 3)  this rank's particle positions  [m]
        masses    : (N,)          ALL particle masses              [kg]
        comm      : MPI communicator

    Returns:
        acc : (local_N, 3)  acceleration of each local particle   [m/s^2]
    """
    # allgather: every rank shares its local positions so every rank
    # has the full (N, 3) position array needed to compute forces
    all_pos_chunks = comm.allgather(local_pos)
    all_pos = np.vstack(all_pos_chunks)   # (N, 3)

    local_N = local_pos.shape[0]
    acc = np.zeros((local_N, 3))

    for i in range(local_N):
        # Vector from particle i to every other particle
        diff = all_pos - local_pos[i]                          # (N, 3)

        # Softened squared distance: + SOFTENING² avoids division by zero
        # when two particles are at the same position
        dist_sq = np.sum(diff**2, axis=1) + SOFTENING**2      # (N,)

        # Cubed distance used in gravitational force formula
        dist_cb = dist_sq ** 1.5                               # (N,)

        # Newton's law: a_i = G * Σ_j  m_j * (r_j - r_i) / |r_j - r_i|³
        acc[i] = G * np.sum(
            masses[:, np.newaxis] * diff / dist_cb[:, np.newaxis],
            axis=0
        )

    return acc