# Quality System Prototype Project

Web prototype for quality management (ISO), with authentication, KPIs, tables, charts, and non-conformities. Backend in **Node.js + Express**, views in **EJS**, database in **JSON** files.

**Source code:** `Protótipo/ER prototype/`

---

## Requirements

| Requirement | Version | Notes |
|-------------|---------|-------|
| [Node.js](https://nodejs.org/) | 18+ (LTS recommended) | Includes `npm` |
| IDE (optional) | — | VS Code, Cursor, etc. |

Verify:

```bash
node --version
npm --version
```

---

## Install dependencies

```powershell
cd "Protótipo\ER prototype"
npm install
```

On Linux/macOS:

```bash
cd "Protótipo/ER prototype"
npm install
```

---

## How to run

### Development mode (with auto-reload)

```powershell
npm run dev
```

### Production mode

```powershell
npm start
```

### Full setup (initialize DB + start)

```powershell
npm run setup
```

Open in browser: **http://localhost:3000**

---

## Test accounts

| Role | Email | Password |
|------|-------|----------|
| Admin | admin@example.com | Admin123! |
| Quality Manager | qm@example.com | Qm12345 |
| Staff | staff@example.com | Staff123 |

Users are defined in `models/users.json`. If login fails, check credentials in that file.

---

## Available scripts

| Command | Description |
|---------|-------------|
| `npm install` | Install dependencies |
| `npm run init` | Initialize the JSON database |
| `npm run dev` | Start with nodemon (auto-reload) |
| `npm start` | Start the server |
| `npm run setup` | `init` + `dev` |

---

## Main dependencies

- `express` — web server
- `ejs` + `express-ejs-layouts` — HTML templates
- `express-session` — user sessions
- `bcrypt` — password hashing
- `multer` — file uploads
- `dotenv` — environment variables
- `nodemon` — auto-reload (dev)

---

## Project structure

```
Projeto_Protótipo_Sistema_Qualidade/
└── Protótipo/ER prototype/
    ├── server.js           ← entry point (port 3000)
    ├── package.json
    ├── routes/
    ├── views/
    ├── models/             ← users.json, JSON data
    └── public/
```

---

## Troubleshooting

| Problem | Solution |
|---------|----------|
| `npm install` fails | Confirm Node.js 18+ is installed |
| Port 3000 in use | Close another app or set `PORT=3001` |
| Login doesn't work | Check `models/users.json` |
| Blank page | Confirm you run `npm run dev` in the correct folder |
