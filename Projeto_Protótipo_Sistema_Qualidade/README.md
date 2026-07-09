# Projeto Protótipo Sistema Qualidade

Protótipo web de gestão da qualidade (ISO), com autenticação, KPIs, tabelas, gráficos e não-conformidades. Backend em **Node.js + Express**, views em **EJS**, base de dados em ficheiros **JSON**.

**Código-fonte:** `Protótipo/ER prototype/`

---

## O que precisa de instalar

| Requisito | Versão | Notas |
|-----------|--------|-------|
| [Node.js](https://nodejs.org/) | 18+ (LTS recomendado) | Inclui `npm` |
| IDE (opcional) | — | VS Code, Cursor, etc. |

Verificar:

```bash
node --version
npm --version
```

---

## Como instalar dependências

```powershell
cd "Protótipo\ER prototype"
npm install
```

No Linux/macOS:

```bash
cd "Protótipo/ER prototype"
npm install
```

---

## Como executar

### Modo desenvolvimento (com auto-reload)

```powershell
npm run dev
```

### Modo produção

```powershell
npm start
```

### Setup completo (inicializar BD + arrancar)

```powershell
npm run setup
```

Abrir no browser: **http://localhost:3000**

---

## Contas de teste

| Papel | Email | Password |
|-------|-------|----------|
| Admin | admin@example.com | Admin123! |
| Quality Manager | qm@example.com | Qm12345 |
| Staff | staff@example.com | Staff123 |

> Os utilizadores estão definidos em `models/users.json`. Em caso de problemas de login, confirme as credenciais nesse ficheiro.

---

## Scripts disponíveis

| Comando | Descrição |
|---------|-----------|
| `npm install` | Instala dependências |
| `npm run init` | Inicializa a base de dados JSON |
| `npm run dev` | Arranca com nodemon (auto-reload) |
| `npm start` | Arranca o servidor |
| `npm run setup` | `init` + `dev` |

---

## Dependências principais

- `express` — servidor web
- `ejs` + `express-ejs-layouts` — templates HTML
- `express-session` — sessões de utilizador
- `bcrypt` — hash de passwords
- `multer` — upload de ficheiros
- `dotenv` — variáveis de ambiente
- `nodemon` — auto-reload (dev)

---

## Estrutura do projeto

```
Projeto_Protótipo_Sistema_Qualidade/
└── Protótipo/ER prototype/
    ├── server.js           ← entry point (porta 3000)
    ├── package.json
    ├── routes/
    ├── views/
    ├── models/             ← users.json, dados JSON
    └── public/
```

---

## Resolução de problemas

| Problema | Solução |
|----------|---------|
| `npm install` falha | Confirme Node.js 18+ instalado |
| Porta 3000 ocupada | Feche outra app ou defina `PORT=3001` |
| Login não funciona | Verifique `models/users.json` |
| Página em branco | Confirme que corre `npm run dev` na pasta correta |
