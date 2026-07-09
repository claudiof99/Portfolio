# Projeto Greenfoot — The Last Of Us

Jogo de plataforma 2D inspirado em *The Last of Us*, desenvolvido em **Java** com o motor **Greenfoot 3.0**.

---

## O que precisa de instalar

| Requisito | Versão | Notas |
|-----------|--------|-------|
| [Greenfoot](https://www.greenfoot.org/download) | 3.0+ | IDE e motor do jogo |
| JDK | Incluído no Greenfoot | Java Development Kit |

> O Greenfoot inclui o JDK necessário. Não é preciso instalar Java separadamente na maioria dos casos.

---

## Como executar

1. Instale o **Greenfoot 3.x** a partir de [greenfoot.org](https://www.greenfoot.org/download).
2. Abra o Greenfoot.
3. **File → Open Project** e selecione a pasta `Projeto_Greenfoot_The Last Of Us`.
4. Clique no botão **Run** (ou prima `Shift+Run` para correr sem pausa).

O jogo arranca no mundo **`TitleScreen`** (ecrã de título com botões Play, Tutorial e Exit).

---

## Controlos

Os controlos são geridos pelas classes `Player`, `GameManager` e `EventListener`. Após carregar em **Play** no ecrã de título, use as teclas definidas no jogo para mover o jogador, saltar e interagir.

---

## Estrutura do projeto

```
Projeto_Greenfoot_The Last Of Us/
├── project.greenfoot       ← ficheiro de projeto Greenfoot
├── TitleScreen.java        ← mundo inicial
├── GameManager.java
├── Player.java
├── Level.java
└── ... (outras classes .java)
```

---

## Notas importantes

### Assets em falta

O código referencia imagens e sons que **podem não estar incluídos** neste repositório, por exemplo:

- `./Title screen/logo.png`
- `./Title screen/starting_screen.jpg`
- `idle_0.png` (sprite do jogador)

Se o jogo abrir mas aparecer sem gráficos, é necessário adicionar a pasta `Title screen/` e os sprites de animação ao projeto.

### Não é um projeto Maven/Gradle

Este projeto **não se compila com `javac` ou Maven** diretamente — deve ser aberto e executado exclusivamente no **Greenfoot IDE**.

---

## Resolução de problemas

| Problema | Solução |
|----------|---------|
| Projeto não abre | Confirme que selecionou a pasta com `project.greenfoot` |
| Imagens em falta | Adicione os assets referenciados nas classes Java |
| Erro de compilação no Greenfoot | Verifique a versão do Greenfoot (3.0.0 referenciada no projeto) |
