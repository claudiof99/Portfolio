# AI Chat Project

Web chat application with user authentication, friend lists, and real-time messaging via **Socket.IO**. Backend in **Node.js + Express**, views in **EJS**, database in **MongoDB Atlas**.

**Entry point:** `BackEnd/server.js`

---

## Current status

The original **MongoDB Atlas cluster no longer exists**, so **login and registration do not work** out of the box. The web server still starts and the login page loads, but authentication requires a new database connection.

---

## Requirements

| Requirement                    | Version               | Notes                                        |
| ------------------------------ | --------------------- | -------------------------------------------- |
| [Node.js](https://nodejs.org/) | 18+ (LTS recommended) | Includes `npm`                               |
| MongoDB Atlas                  | —                     | Free tier works; required for login/register |
| IDE (optional)                 | —                     | VS Code, Cursor, etc.                        |

Verify:

```bash
node --version
npm --version
```

---

## Install dependencies

```powershell
cd BackEnd
npm install
```

On Linux/macOS:

```bash
cd BackEnd
npm install
```

---

## Configure MongoDB

1. Create a free cluster at [MongoDB Atlas](https://www.mongodb.com/cloud/atlas).
2. Create a database user and allow your IP address in **Network Access**.
3. Copy your connection string.
4. Update the connection string in `BackEnd/server.js` (the `mongoose.connect(...)` call near the bottom of the file).

Example format:

```
mongodb+srv://<username>:<password>@<cluster>.mongodb.net/<dbname>?retryWrites=true&w=majority
```

Without a working MongoDB connection, the server starts but you will see a connection error and login will fail.

---

## How to run

### Production mode

```powershell
cd BackEnd
npm start
```

### Development mode (auto-reload)

```powershell
cd BackEnd
npm run dev
```

Open in browser: **http://localhost:3050**

---

## Routes

| URL         | Description                              |
| ----------- | ---------------------------------------- |
| `/`         | Login page                               |
| `/Login`    | Login                                    |
| `/Register` | User registration                        |
| `/MainPage` | Main chat page (requires authentication) |

---

## Main dependencies

| Package                                                   | Purpose           |
| --------------------------------------------------------- | ----------------- |
| `express`                                                 | Web server        |
| `ejs`                                                     | HTML templates    |
| `mongoose`                                                | MongoDB ODM       |
| `passport` + `passport-local` + `passport-local-mongoose` | Authentication    |
| `express-session`                                         | User sessions     |
| `socket.io`                                               | Real-time chat    |
| `nodemon`                                                 | Auto-reload (dev) |

---

## Project structure

```
Projeto_Chat_IA/
├── BackEnd/
│   ├── server.js              ← entry point (port 3050)
│   ├── package.json
│   ├── Models/user.js
│   ├── Controllers/
│   └── Routes/
├── FrontEnd/
│   ├── views/                 ← Login, Register, MainPage (EJS)
│   └── public/                ← CSS and JavaScript
└── Nota sobre o Projeto.txt
```

---

## Troubleshooting

| Problem                          | Solution                                                                 |
| -------------------------------- | ------------------------------------------------------------------------ |
| `ENOTFOUND` MongoDB error        | The old Atlas cluster is gone — create a new one and update `server.js`  |
| Login does nothing / fails       | Confirm MongoDB is connected (`Connected` should appear in the terminal) |
| Port 3050 in use                 | Stop the other process or change `PORT` in `server.js`                   |
| `npm install` fails              | Confirm Node.js 18+ is installed                                         |
| Page loads but chat doesn't work | Log in first; Socket.IO chat requires an authenticated session           |

---

## Making the project fully functional again

1. Set up a new **MongoDB Atlas** cluster.
2. Update the connection string in `BackEnd/server.js`.
3. Run `npm start` in `BackEnd/`.
4. Open **http://localhost:3050** and **register a new user** (old users were on the deleted database).
5. Use the main page to add friends and chat in real time.
