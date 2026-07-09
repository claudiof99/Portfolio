# Civilizations Game Project

Turn-based strategy game in **Java** with a rectangular map, civilizations (Roman, Greek, etc.), and a command-line interface.

**Source code:** `Projeto2/Civilizations/`

---

## Requirements

| Requirement | Version | Notes |
|-------------|---------|-------|
| JDK | **23** | Per `pom.xml` (`maven.compiler.release=23`) |
| Maven | 3.6+ | Dependency and build manager |

Verify:

```bash
java --version    # expected: 23.x
mvn --version
```

---

## How to build

```powershell
cd Projeto2\Civilizations
mvn compile
```

On Linux/macOS:

```bash
cd Projeto2/Civilizations
mvn compile
```

---

## How to run

### Option A — Maven

```powershell
cd Projeto2\Civilizations
mvn compile exec:java -Dexec.mainClass="com.mycompany.poo.civilizations.Civilizations"
```

### Option B — Java directly

```powershell
cd Projeto2\Civilizations
mvn compile
java -cp target/classes com.mycompany.poo.civilizations.Civilizations
```

### Option C — IDE

Open the `Projeto2/Civilizations` folder in IntelliJ IDEA, VS Code, or NetBeans and run the `Civilizations` class.

---

## Dependencies

| Library | Version | Purpose |
|---------|---------|---------|
| `org.fusesource.jansi:jansi` | 2.4.0 | Terminal colors |

Maven downloads dependencies automatically with `mvn compile`.

---

## Usage

The game starts with an interactive terminal menu:

1. Choose a civilization (Roman, Greek, etc.)
2. Manage units, resources, and turns on a 33×15 map
3. Follow the on-screen instructions

---

## Project structure

```
Projeto_Jogo_Civilizations/
└── Projeto2/Civilizations/
    ├── pom.xml
    └── src/main/java/com/mycompany/poo/civilizations/
        └── Civilizations.java    ← entry point
```

---

## Troubleshooting

| Problem | Solution |
|---------|----------|
| `release version 23 not supported` | Install JDK 23 |
| `mvn` not found | Install Maven and add it to PATH |
| Garbled terminal characters | Jansi requires a compatible terminal (Windows Terminal, PowerShell, Linux) |
