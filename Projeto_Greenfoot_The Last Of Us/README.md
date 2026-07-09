# Greenfoot Project — The Last Of Us

2D platform game inspired by *The Last of Us*, developed in **Java** with the **Greenfoot 3.0** engine.

---

## Requirements

| Requirement | Version | Notes |
|-------------|---------|-------|
| [Greenfoot](https://www.greenfoot.org/download) | 3.0+ | Game IDE and engine |
| JDK | Included with Greenfoot | Java Development Kit |

Greenfoot includes the required JDK. You usually do not need to install Java separately.

---

## How to run

1. Install **Greenfoot 3.x** from [greenfoot.org](https://www.greenfoot.org/download).
2. Open Greenfoot.
3. **File → Open Project** and select the `Projeto_Greenfoot_The Last Of Us` folder.
4. Click **Run** (or press `Shift+Run` to run without pausing).

The game starts in the **`TitleScreen`** world (title screen with Play, Tutorial, and Exit buttons).

---

## Controls

Controls are handled by the `Player`, `GameManager`, and `EventListener` classes. After clicking **Play** on the title screen, use the in-game keys to move the player, jump, and interact.

---

## Project structure

```
Projeto_Greenfoot_The Last Of Us/
├── project.greenfoot       ← Greenfoot project file
├── TitleScreen.java        ← initial world
├── GameManager.java
├── Player.java
├── Level.java
└── ... (other .java classes)
```

---

## Important notes

### Missing assets

The code references images and sounds that **may not be included** in this repository, for example:

- `./Title screen/logo.png`
- `./Title screen/starting_screen.jpg`
- `idle_0.png` (player sprite)

If the game opens but shows no graphics, add the `Title screen/` folder and animation sprites to the project.

### Not a Maven/Gradle project

This project **cannot be compiled with `javac` or Maven** directly — it must be opened and run exclusively in the **Greenfoot IDE**.

---

## Troubleshooting

| Problem | Solution |
|---------|----------|
| Project won't open | Make sure you selected the folder containing `project.greenfoot` |
| Missing images | Add the assets referenced in the Java classes |
| Greenfoot compile error | Check Greenfoot version (3.0.0 referenced in the project) |
