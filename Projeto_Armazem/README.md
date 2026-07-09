# Warehouse Project

C++17 warehouse management simulator. The program generates random sections, manages parts inventory, billing, and interactive operations via the command line.

**Source code:** `Armazem/Warehouse/`

---

## Requirements

| Requirement | Version | Notes |
|-------------|---------|-------|
| C++ compiler | C++17 | GCC, Clang, or MSVC |
| CMake | ≥ 3.27 | Build generator |
| IDE (optional) | — | CLion, VS Code, Visual Studio |

---

## How to build

```powershell
cd Armazem\Warehouse
cmake -B cmake-build-debug
cmake --build cmake-build-debug
```

On Linux/macOS:

```bash
cd Armazem/Warehouse
cmake -B cmake-build-debug
cmake --build cmake-build-debug
```

The generated executable is **`Warehouse`** (or `Warehouse.exe` on Windows).

---

## How to run

No command-line arguments are required.

```powershell
cd Armazem\Warehouse\cmake-build-debug
.\Warehouse.exe
```

On Linux/macOS:

```bash
cd Armazem/Warehouse/cmake-build-debug
./Warehouse
```

The program starts with an interactive terminal menu to manage sections, parts, and billing.

---

## Project structure

```
Projeto_Armazem/
└── Armazem/Warehouse/
    ├── main.cpp
    ├── warehouse.cpp / warehouse.h
    └── CMakeLists.txt
```
