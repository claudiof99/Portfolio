# Projeto Aeroporto

Simulador de gestão de aeroporto em C++17. O programa gere listas de aviões (aproximação, pista e descolagem), passageiros, árvores de pesquisa e gravação de estado em ficheiros.

**Código-fonte:** `Airport_123/Airport_123/`

---

## O que precisa de instalar

| Requisito | Versão | Notas |
|-----------|--------|-------|
| Compilador C++ | C++17 | GCC, Clang ou MSVC |
| CMake | ≥ 3.27 | Gerador de build |
| IDE (opcional) | — | CLion, VS Code, Visual Studio |

---

## Como compilar

Abra um terminal na pasta do projeto:

```powershell
cd Airport_123\Airport_123
cmake -B cmake-build-debug
cmake --build cmake-build-debug
```

No Linux/macOS:

```bash
cd Airport_123/Airport_123
cmake -B cmake-build-debug
cmake --build cmake-build-debug
```

O executável gerado chama-se **`Airport`** (ou `Airport.exe` no Windows).

---

## Como executar

O programa precisa de **3 argumentos** (ficheiros de estado das listas de aviões) e de **ficheiros de dados** no diretório de trabalho.

### Ficheiros de dados obrigatórios (no diretório atual)

Estes ficheiros `.txt` têm de existir na pasta onde corre o executável:

- `primeiro_nome.txt`
- `segundo_nome.txt`
- `nacionalidade.txt`
- `voo.txt`
- `modelo.txt`
- `origem.txt`
- `destino.txt`

Exemplos destes ficheiros estão em `Airport_123/Airport_123/cmake-build-debug/`.

### Comando

```powershell
cd Airport_123\Airport_123\cmake-build-debug
.\Airport.exe lista_AvProximacao.txt lista_AvPista.txt lista_AvDescolagem.txt
```

No Linux/macOS:

```bash
cd Airport_123/Airport_123/cmake-build-debug
./Airport lista_AvProximacao.txt lista_AvPista.txt lista_AvDescolagem.txt
```

| Argumento | Descrição |
|-----------|-----------|
| `arquivo_proximacao` | Ficheiro com o estado dos aviões em aproximação |
| `arquivo_pista` | Ficheiro com o estado dos aviões na pista |
| `arquivo_descolagem` | Ficheiro com o estado dos aviões a descolar |

> Se o ficheiro de descolagem ainda não existir, pode criar um ficheiro vazio antes de executar. O programa grava o estado ao sair.

---

## Utilização

Após arrancar, o programa apresenta um menu interativo no terminal para:

- Simular chegadas e movimentos de aviões
- Consultar passageiros por pista, aproximação ou descolagem
- Pesquisar e ordenar passageiros
- Gravar e carregar o estado do aeroporto

---

## Estrutura do projeto

```
Projeto_Aeroporto/
└── Airport_123/Airport_123/
    ├── main.cpp
    ├── airport.cpp / airport.h
    └── CMakeLists.txt
```
