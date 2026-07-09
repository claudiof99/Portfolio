# Airport Project

C++17 airport management simulator. The program manages aircraft lists (approach, runway, and takeoff), passengers, search trees, and state persistence to files.

**Source code:** `Airport_123/Airport_123/`

---

## Requirements

| Requirement | Version | Notes |
|-------------|---------|-------|
| C++ compiler | C++17 | GCC, Clang, or MSVC |
| CMake | ≥ 3.27 | Build generator |
| IDE (optional) | — | CLion, VS Code, Visual Studio |

---

## How to build

Open a terminal in the project folder:

```powershell
cd Airport_123\Airport_123
cmake -B cmake-build-debug
cmake --build cmake-build-debug
```

On Linux/macOS:

```bash
cd Airport_123/Airport_123
cmake -B cmake-build-debug
cmake --build cmake-build-debug
```

The generated executable is **`Airport`** (or `Airport.exe` on Windows).

---

## How to run

The program requires **3 arguments** (aircraft list state files) and **data files** in the working directory.

### Required data files (in the current directory)

These `.txt` files must exist in the folder where you run the executable:

- `primeiro_nome.txt`
- `segundo_nome.txt`
- `nacionalidade.txt`
- `voo.txt`
- `modelo.txt`
- `origem.txt`
- `destino.txt`

Sample files are available in `Airport_123/Airport_123/cmake-build-debug/`.

### Command

```powershell
cd Airport_123\Airport_123\cmake-build-debug
.\Airport.exe lista_AvProximacao.txt lista_AvPista.txt lista_AvDescolagem.txt
```

On Linux/macOS:

```bash
cd Airport_123/Airport_123/cmake-build-debug
./Airport lista_AvProximacao.txt lista_AvPista.txt lista_AvDescolagem.txt
```

| Argument | Description |
|----------|-------------|
| `arquivo_proximacao` | File with approach aircraft state |
| `arquivo_pista` | File with runway aircraft state |
| `arquivo_descolagem` | File with takeoff aircraft state |

If the takeoff file does not exist yet, create an empty file before running. The program saves state on exit.

---

## Usage

After starting, the program shows an interactive terminal menu to:

- Simulate aircraft arrivals and movements
- Query passengers by runway, approach, or takeoff
- Search and sort passengers
- Save and load airport state

---

## Project structure

```
Projeto_Aeroporto/
└── Airport_123/Airport_123/
    ├── main.cpp
    ├── airport.cpp / airport.h
    └── CMakeLists.txt
```
