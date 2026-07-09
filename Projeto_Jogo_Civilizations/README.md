# Projeto Jogo Civilizations

Jogo de estratégia por turnos em **Java** com mapa retangular, civilizações (Romana, Grega, etc.) e interface de linha de comandos.

**Código-fonte:** `Projeto2/Civilizations/`

---

## O que precisa de instalar

| Requisito | Versão | Notas |
|-----------|--------|-------|
| JDK | **23** | Conforme `pom.xml` (`maven.compiler.release=23`) |
| Maven | 3.6+ | Gestor de dependências e build |

Verificar:

```bash
java --version    # esperado: 23.x
mvn --version
```

---

## Como compilar

```powershell
cd Projeto2\Civilizations
mvn compile
```

No Linux/macOS:

```bash
cd Projeto2/Civilizations
mvn compile
```

---

## Como executar

### Opção A — Maven

```powershell
cd Projeto2\Civilizations
mvn compile exec:java -Dexec.mainClass="com.mycompany.poo.civilizations.Civilizations"
```

### Opção B — Java direto

```powershell
cd Projeto2\Civilizations
mvn compile
java -cp target/classes com.mycompany.poo.civilizations.Civilizations
```

### Opção C — IDE

Abra a pasta `Projeto2/Civilizations` no IntelliJ IDEA, VS Code ou NetBeans e execute a classe `Civilizations`.

---

## Dependências

| Biblioteca | Versão | Uso |
|------------|--------|-----|
| `org.fusesource.jansi:jansi` | 2.4.0 | Cores no terminal |

O Maven descarrega automaticamente as dependências com `mvn compile`.

---

## Utilização

O jogo arranca com um menu interativo no terminal:

1. Escolha uma civilização (Romana, Grega, etc.)
2. Gerencie unidades, recursos e turnos no mapa 33×15
3. Siga as instruções apresentadas no ecrã

---

## Estrutura do projeto

```
Projeto_Jogo_Civilizations/
└── Projeto2/Civilizations/
    ├── pom.xml
    └── src/main/java/com/mycompany/poo/civilizations/
        └── Civilizations.java    ← entry point
```

---

## Resolução de problemas

| Problema | Solução |
|----------|---------|
| `release version 23 not supported` | Instale JDK 23 |
| `mvn` não encontrado | Instale Maven e adicione ao PATH |
| Caracteres estranhos no terminal | Jansi requer terminal compatível (Windows Terminal, PowerShell, Linux) |
