# OS Project — Client/Server with Sockets

Client/server game in **C** using **UNIX domain sockets**, `pthread`, and `epoll`. The server manages game state; the client connects and plays in single-player mode.

---

## Requirements

| Requirement | Notes |
|-------------|-------|
| **Linux, macOS, or WSL** | Uses POSIX APIs (`epoll`, `pthread`, UNIX sockets) — **does not compile natively on Windows** |
| GCC | C compiler |
| Make | Build tool |

On Ubuntu/Debian (or WSL):

```bash
sudo apt install build-essential
```

Verify:

```bash
gcc --version
make --version
```

---

## How to build

```bash
cd Projeto_SO
make
```

This produces two executables in `Build/`:

| Executable | Description |
|------------|-------------|
| `Build/servidor_exec` | Game server |
| `Build/cliente_exec` | Game client |

### Debug build

```bash
make debug
```

Produces `Build/servidor_exec_debug` and `Build/cliente_exec_debug` with AddressSanitizer.

### Clean

```bash
make clean
```

---

## How to run

You need **two terminals**:

**Terminal 1 — Server:**

```bash
cd Projeto_SO
./Build/servidor_exec
```

**Terminal 2 — Client:**

```bash
cd Projeto_SO
./Build/cliente_exec
```

The UNIX socket uses the path `/tmp/s.unixstr2186622_test` (defined in `unix.h`). The server must start before the client.

---

## Project structure

```
Projeto_SO/
├── Makefile
├── unix.h
├── util.c / log.c
├── Servidor/
│   ├── socketsSetupServidorSinglePlayer.c
│   ├── socketsUtilsServidor.c
│   └── servidorGameManager.c
└── Cliente/
    ├── socketsSetupClienteSinglePlayer.c
    ├── socketsUtilsCliente.c
    └── singlePlayerSolucaoCompleta.c
```

---

## Running on Windows (via WSL)

1. Install [WSL2](https://learn.microsoft.com/en-us/windows/wsl/install) with Ubuntu.
2. Access the project inside WSL:

```bash
cd /mnt/c/Users/<user>/.../Portfolio/Projeto_SO
make
./Build/servidor_exec
```

3. In another WSL terminal:

```bash
./Build/cliente_exec
```

---

## Troubleshooting

| Problem | Solution |
|---------|----------|
| `make` fails on Windows | Use WSL — the code is not compatible with native MSVC |
| Client won't connect | Confirm the server is running first |
| Socket error | Check if `/tmp/s.unixstr2186622_test` is blocked by a previous process |
| Sanitizer compile errors | Install `libasan` (`sudo apt install libasan6`) |
