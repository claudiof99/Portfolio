"""
plotting.py - Galaxy Frame Visualization

Saves a single PNG frame with two subplots:
  - Left:  2D top-down view (X-Y plane)
  - Right: 3D perspective view

Called by rank 0 only, every PLOT_EVERY steps.
No MPI needed here — just numpy arrays and matplotlib.

Changes from original:
  - save_frame() now also receives sim_time_yr and total_KE for richer titles
  - Colour-maps particles by cylindrical radius (visually nicer than plain white)
  - Fixed title: shows both step number and true simulated time in years

Test standalone with:
    python plotting.py
"""

import numpy as np
import matplotlib
matplotlib.use("Agg")   # non-interactive backend — works on all OS / HPC
import matplotlib.pyplot as plt
import os


def save_frame(
    positions:    np.ndarray,
    step:         int,
    sim_time_yr:  float,
    KEs:       np.ndarray,
    total_KE:    float,
    output_dir:   str
) -> None:
    """
    Save a 2-D + 3-D plot of the galaxy as a PNG frame.

    Args:
        positions   : (N, 3)  all particle positions in metres
        step        : current simulation step number
        sim_time_yr : simulated time in years  (= step * dt_years)
        KEs         : kinetic energy for each particle [J]
        total_KE    : total kinetic energy across all particles [J]
        output_dir  : directory to save the PNG file
    """
    os.makedirs(output_dir, exist_ok=True)

    x, y, z = positions[:, 0], positions[:, 1], positions[:, 2]



    # Subsample for plotting if N is very large (plotting 10^8 points is slow)
    MAX_PLOT = 50000
    if len(x) > MAX_PLOT:
        idx = np.random.choice(len(x), MAX_PLOT, replace=False)
        x, y, z = x[idx], y[idx], z[idx]
        KEs = KEs[idx]

    # Colour particles by energy for a nicer visual
    energy_norm = (KEs - KEs.min()) / (KEs.max() - KEs.min() + 1e-30)

    # Limit based on 90th percentile of 3D distance for consistent scaling
    
    lim = np.percentile(np.sqrt(x**2 + y**2 + z**2), 90) * 1.5

    fig = plt.figure(figsize=(14, 6), facecolor="black")
    fig.suptitle(
        f"Galaxy Simulation  —  Step {step}  |  "
        f"Time: {sim_time_yr:,.0f} yr  |  "
        f"KE: {total_KE:.2e} J",
        color="white", fontsize=12, y=0.98
    )

    # -- Left subplot: 2-D top-down view (X-Y plane) -------------------------
    ax1 = fig.add_subplot(1, 2, 1)
    ax1.set_facecolor("black")
    sc1 = ax1.scatter(x, y, s=3, c=energy_norm, cmap="plasma", alpha=0.9,
                      linewidths=0)
    ax1.set_xlim(-lim, lim)
    ax1.set_ylim(-lim, lim)
    ax1.set_xlabel("X  (m)", color="white", fontsize=9)
    ax1.set_ylabel("Y  (m)", color="white", fontsize=9)
    ax1.set_title("Top View  (X-Y plane)", color="white", fontsize=10)
    ax1.tick_params(colors="white", labelsize=7)
    for spine in ax1.spines.values():
        spine.set_edgecolor("grey")
    # Colour bar
    cbar1 = plt.colorbar(sc1, ax=ax1, fraction=0.03, pad=0.04)
    cbar1.set_label("Normalised kinetic energy", color="white", fontsize=7)
    cbar1.ax.yaxis.set_tick_params(color="white", labelsize=6)
    plt.setp(cbar1.ax.yaxis.get_ticklabels(), color="white")

    # -- Right subplot: 3-D perspective view --------------------------------
    ax2 = fig.add_subplot(1, 2, 2, projection="3d")
    ax2.set_facecolor("black")
    ax2.scatter(x, y, z, s=3, c=energy_norm, cmap="plasma", alpha=0.8,
                linewidths=0)
    ax2.set_xlim(-lim, lim)
    ax2.set_ylim(-lim, lim)
    ax2.set_zlim(-lim * 0.1, lim * 0.1)
    ax2.set_xlabel("X", color="white", labelpad=2, fontsize=8)
    ax2.set_ylabel("Y", color="white", labelpad=2, fontsize=8)
    ax2.set_zlabel("Z", color="white", labelpad=2, fontsize=8)
    ax2.set_title("3-D Perspective View", color="white", fontsize=10)
    ax2.tick_params(colors="white", labelsize=6)
    ax2.xaxis.pane.fill = False
    ax2.yaxis.pane.fill = False
    ax2.zaxis.pane.fill = False
    ax2.xaxis.pane.set_edgecolor("grey")
    ax2.yaxis.pane.set_edgecolor("grey")
    ax2.zaxis.pane.set_edgecolor("grey")

    plt.tight_layout(rect=[0, 0, 1, 0.96])

    filename = os.path.join(output_dir, f"frame_{step:07d}.png")
    plt.savefig(filename, dpi=80, facecolor="black")
    plt.close(fig)

