# Galaxy Simulation (MPI)

Parallel N-body galaxy simulation in Python. Multiple MPI processes compute gravitational forces, advance particles over time, save PNG frames, and assemble an animated GIF at the end.

## What you need

| Requirement | Required? | Notes |
|-------------|-----------|-------|
| Python 3.9+ | Yes | Check with `python --version` |
| MPI runtime | Yes | MS-MPI (Windows), OpenMPI (macOS/Linux) |
| Python packages | Yes | `numpy`, `mpi4py`, `matplotlib`, `pillow` |
| FFmpeg | Recommended | Faster, higher-quality GIFs. Falls back to Pillow if missing. |

## Setup

### 1. Install Python

Download and install Python 3 from [python.org](https://www.python.org/downloads/). On Windows, enable **"Add Python to PATH"** during installation.

### 2. Install an MPI runtime

**Windows** — install [Microsoft MPI (MS-MPI)](https://www.microsoft.com/en-us/download/details.aspx?id=105289), or:

```powershell
winget install Microsoft.MPI
```

After installing, open a **new** terminal and confirm `mpiexec` works:

```powershell
mpiexec
```

**macOS** — install OpenMPI via Homebrew:

```bash
brew install openmpi
```

**Linux** — install OpenMPI:

```bash
sudo apt install openmpi-bin openmpi-common libopenmpi-dev   # Debian/Ubuntu
```

### 3. Install Python dependencies

Open a terminal in the project folder and run:

```bash
pip install numpy mpi4py matplotlib pillow
```

> **Windows note:** If `pip install mpi4py` fails, install MS-MPI first (step 2), restart the terminal, then retry.

### 4. Install FFmpeg (recommended)

Produces better GIFs than the Pillow fallback.

**Windows:**

```powershell
winget install -e --id Gyan.FFmpeg
```

**macOS:**

```bash
brew install ffmpeg
```

**Linux:**

```bash
sudo apt install ffmpeg
```

## How to run

Open a terminal in the project directory:

```bash
cd path/to/Projeto_Galaxia_Simulação_Paralelismo
```

`-n <N>` sets the number of MPI processes (ranks). Use a value up to your CPU core count.

### Quick test (~1–2 minutes)

**Windows:**

```powershell
mpiexec -n 4 python main.py --particles 1000 --steps 50 --dt-max 1000000 --plot-every 5000000
```

**macOS / Linux:**

```bash
mpirun -n 4 python main.py --particles 1000 --steps 50 --dt-max 1000000 --plot-every 5000000 --oversubscribe
```

### Full run (from the project report)

**Windows:**

```powershell
mpiexec -n 4 python main.py --particles 7500 --steps 200 --dt-max 500000000 --plot-every 500000000
```

**macOS / Linux:**

```bash
mpirun -n 4 python main.py --particles 7500 --steps 200 --dt-max 500000000 --plot-every 500000000 --oversubscribe
```

## More example commands

**Windows:**

```powershell
mpiexec -n 4 python main.py --particles 7500 --steps 200 --dt-max 500000000 --plot-every 500000000

mpiexec -n 2 python main.py --particles 1000 --steps 10 --dt-max 500000000 --plot-every 500000000

mpiexec -n 3 python main.py --particles 10000 --steps 5 --dt-max 500000000 --plot-every 500000000

mpiexec -n 1 python main.py --particles 100000 --steps 1 --dt-max 500000000 --plot-every 500000000
```

**macOS / Linux:**

```bash
mpirun -n 4 python main.py --particles 20000 --steps 100 --dt-max 500000000 --plot-every 500000000 --oversubscribe

mpirun -n 4 python main.py --particles 7500 --steps 200 --dt-max 500000000 --plot-every 500000000 --oversubscribe

mpirun -n 2 python main.py --particles 1000 --steps 10 --dt-max 500000000 --plot-every 500000000 --oversubscribe

mpirun -n 3 python main.py --particles 10000 --steps 5 --dt-max 500000000 --plot-every 500000000 --oversubscribe

mpirun -n 1 python main.py --particles 100000 --steps 1 --dt-max 500000000 --plot-every 500000000 --oversubscribe
```

## Command-line options

| Flag | Default | Description |
|------|---------|-------------|
| `--particles` | 10000 | Total number of star particles |
| `--steps` | 10000 | Number of simulation time steps |
| `--dt-max` | 1000 | Maximum time step size (years) |
| `--dt-min` | 10.0 | Minimum time step size (years) |
| `--plot-every` | 1000 | Save a frame every N simulated years |
| `--frames-dir` | `frames` | Directory for PNG frames |
| `--output-gif` | `output/galaxy.gif` | Output GIF path |

## Output files

After a successful run you should see:

| File / folder | Description |
|---------------|-------------|
| `frames/` | PNG snapshots saved during the simulation |
| `output/galaxy.gif` | Animated GIF built at the end |
| `timing_results.txt` | Wall-clock timing per run (appended) |
| `energy_log.txt` | Kinetic energy over time |

## Troubleshooting

| Problem | Solution |
|---------|----------|
| `mpiexec` / `mpirun` not found | Install the MPI runtime (step 2) and open a new terminal |
| `No module named 'mpi4py'` | Run `pip install mpi4py` |
| `mpi4py` install fails on Windows | Install MS-MPI first, restart terminal, retry |
| Simulation is very slow | Reduce `--particles` and `--steps`, or increase `--plot-every` |
| No GIF created | Check that `frames/` contains PNG files; if empty, the run may have failed early |
| FFmpeg not used | Install FFmpeg (step 4); otherwise Pillow is used automatically |
