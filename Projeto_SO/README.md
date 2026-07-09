# Projeto SO — Cliente/Servidor com Sockets

Jogo cliente/servidor em **C** com comunicação via **UNIX domain sockets**, `pthread` e `epoll`. O servidor gere o estado do jogo; o cliente liga-se e joga em modo single-player.

---

## O que precisa de instalar

| Requisito | Notas |
|-----------|-------|
| **Linux, macOS ou WSL** | O projeto usa APIs POSIX (`epoll`, `pthread`, UNIX sockets) — **não compila nativamente no Windows** |
| GCC | Compilador C |
| Make | Ferramenta de build |

No Ubuntu/Debian (ou WSL):

```bash
sudo apt install build-essential
```

Verificar:

```bash
gcc --version
make --version
```

---

## Como compilar

```bash
cd Projeto_SO
make
```

Isto gera dois executáveis em `Build/`:

| Executável | Descrição |
|------------|-----------|
| `Build/servidor_exec` | Servidor do jogo |
| `Build/cliente_exec` | Cliente do jogo |

### Versão debug

```bash
make debug
```

Gera `Build/servidor_exec_debug` e `Build/cliente_exec_debug` com AddressSanitizer.

### Limpar

```bash
make clean
```

---

## Como executar

São necessários **dois terminais**:

**Terminal 1 — Servidor:**

```bash
cd Projeto_SO
./Build/servidor_exec
```

**Terminal 2 — Cliente:**

```bash
cd Projeto_SO
./Build/cliente_exec
```

> O socket UNIX usa o caminho `/tmp/s.unixstr2186622_test` (definido em `unix.h`). O servidor deve arrancar antes do cliente.

---

## Estrutura do projeto

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

## Executar no Windows (via WSL)

1. Instale [WSL2](https://learn.microsoft.com/en-us/windows/wsl/install) com Ubuntu.
2. Copie ou aceda ao projeto dentro do WSL:

```bash
cd /mnt/c/Users/<user>/.../Portfolio/Projeto_SO
make
./Build/servidor_exec
```

3. Noutro terminal WSL:

```bash
./Build/cliente_exec
```

---

## Resolução de problemas

| Problema | Solução |
|----------|---------|
| `make` falha no Windows | Use WSL — o código não é compatível com MSVC nativo |
| Cliente não liga | Confirme que o servidor está a correr primeiro |
| Erro de socket | Verifique se `/tmp/s.unixstr2186622_test` não está bloqueado por processo anterior |
| Erros de compilação com sanitizer | Instale `libasan` (`sudo apt install libasan6`) |
