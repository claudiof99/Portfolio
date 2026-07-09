# Projeto Armazém

Simulador de gestão de armazém em C++17. O programa gera secções aleatórias, gere stock de peças, faturação e operações interativas via linha de comandos.

**Código-fonte:** `Armazem/Warehouse/`

---

## O que precisa de instalar

| Requisito | Versão | Notas |
|-----------|--------|-------|
| Compilador C++ | C++17 | GCC, Clang ou MSVC |
| CMake | ≥ 3.27 | Gerador de build |
| IDE (opcional) | — | CLion, VS Code, Visual Studio |

---

## Como compilar

```powershell
cd Armazem\Warehouse
cmake -B cmake-build-debug
cmake --build cmake-build-debug
```

No Linux/macOS:

```bash
cd Armazem/Warehouse
cmake -B cmake-build-debug
cmake --build cmake-build-debug
```

O executável gerado chama-se **`Warehouse`** (ou `Warehouse.exe` no Windows).

---

## Como executar

Não são necessários argumentos na linha de comandos.

```powershell
cd Armazem\Warehouse\cmake-build-debug
.\Warehouse.exe
```

No Linux/macOS:

```bash
cd Armazem/Warehouse/cmake-build-debug
./Warehouse
```

O programa arranca com um menu interativo no terminal para gerir secções, peças e faturação.

---

## Estrutura do projeto

```
Projeto_Armazem/
└── Armazem/Warehouse/
    ├── main.cpp
    ├── warehouse.cpp / warehouse.h
    └── CMakeLists.txt
```
