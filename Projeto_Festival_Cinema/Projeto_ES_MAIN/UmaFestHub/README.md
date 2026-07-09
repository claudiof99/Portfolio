# UmaFestHub

Festival platform for passes, rentals, film sessions, reviews, awards, and personal lists.

This guide is for **setting up the project on a new machine**.

---

## What you must install

Install these **before** running the app:

| Tool                                                              | Version           | Why                         |
| ----------------------------------------------------------------- | ----------------- | --------------------------- |
| [.NET SDK](https://dotnet.microsoft.com/download)                 | **10.0** or newer | App targets `net10.0`       |
| [Docker Desktop](https://www.docker.com/products/docker-desktop/) | Latest            | Runs MySQL locally          |
| EF Core CLI                                                       | Latest            | Applies database migrations |

Check installations:

```bash
dotnet --version
# Expected: 10.0.x

docker --version
docker compose version
```

Install the EF Core global tool (once per machine):

```bash
dotnet tool install -g dotnet-ef
```

If already installed, update it:

```bash
dotnet tool update -g dotnet-ef
```

---

## Project layout

Open a terminal in the **`UmaFestHub`** folder (where `UmaFestHub.sln` and `docker-compose.yml` live):

```
UmaFestHub/
├── .env                    ← required (see below)
├── docker-compose.yml      ← MySQL + phpMyAdmin
├── UmaFestHub.sln
└── src/
    └── UmaFestHub.Web/     ← web app entry point
```

---

## Step 1 — Create the `.env` file

The app reads database and API settings from a **`.env` file in the `UmaFestHub` root**.

Create `UmaFestHub/.env` with:

```env
ASPNETCORE_ENVIRONMENT=Development
DB_HOST=127.0.0.1
DB_PORT=3307
DB_NAME=UmaFestHub_DB
DB_USER=root
DB_PASSWORD=umafesthub_root
TMDB_API_KEY=your_tmdb_api_key_here
```

- **`DB_*`** values must match `docker-compose.yml` (defaults above work out of the box).
- **`TMDB_API_KEY`** is required to **import films from TMDb**. Get a free key at [themoviedb.org/settings/api](https://www.themoviedb.org/settings/api). Without it, the rest of the app still runs, but film import will fail.

---

## Step 2 — Start MySQL (Docker)

1. **Start Docker Desktop** and wait until it is fully running.
2. From the **`UmaFestHub`** folder:

```bash
docker compose up -d
```

Verify containers are up:

```bash
docker compose ps
```

You should see `mysql` and `phpmyadmin` running.

| Service               | URL                   | Credentials                             |
| --------------------- | --------------------- | --------------------------------------- |
| MySQL                 | `127.0.0.1:3307`      | user `root`, password `umafesthub_root` |
| phpMyAdmin (optional) | http://localhost:8081 | same as above                           |

---

## Step 3 — Restore packages

From the **`UmaFestHub`** folder:

```bash
dotnet restore UmaFestHub.sln
```

---

## Step 4 — Create / update the database

Still in **`UmaFestHub`**, apply EF Core migrations (creates tables + seed data):

```bash
dotnet ef database update --project src/UmaFestHub.Infrastructure/UmaFestHub.Infrastructure.csproj --startup-project src/UmaFestHub.Web/UmaFestHub.Web.csproj
```

This step reads `.env` for the connection string. If it fails with `DB_PASSWORD was not found`, your `.env` file is missing or not in the project root.

---

## Step 5 — Run the app

```bash
dotnet run --project src/UmaFestHub.Web/UmaFestHub.Web.csproj
```

Open: **http://localhost:5050**

(Port is set in `src/UmaFestHub.Web/appsettings.json`.)

---

## Test accounts (seeded automatically)

| Role      | Email                    | Password      |
| --------- | ------------------------ | ------------- |
| Admin     | admin@umafesthub.com     | Admin@123     |
| Organizer | organizer@umafesthub.com | Organizer@123 |
| Customer  | customer@umafesthub.com  | Customer@123  |

---

## Seeded demo data

After migrations, the database includes:

- **Festival:** Uma Spring Fest (April 20–27, 2026)
- **Film:** Midnight Frames (112 min)

---

## Quick smoke test

1. Log in as **Organizer** → create or open a festival.
2. Log in as **Customer** → browse, add a pass to cart, checkout.
3. All dates/times in the app use **UTC** — when creating sessions, use current UTC time for immediate testing.

---

## Troubleshooting

### `dotnet --version` is not 10.0.x

Install [.NET SDK 10](https://dotnet.microsoft.com/download). Older SDKs cannot build this project.

### Docker / MySQL connection errors

- Confirm Docker Desktop is running: `docker compose ps`
- Restart database: `docker compose down` then `docker compose up -d`
- Check `.env`: `DB_PORT=3307` (host port mapped in `docker-compose.yml`)

### `dotnet ef` not found

```bash
dotnet tool install -g dotnet-ef
```

Close and reopen the terminal, then retry Step 4.

### Port already in use

- **5050** — another app is using the web port; stop it or change `"urls"` in `appsettings.json`.
- **3307** — another MySQL instance; change the host port in `docker-compose.yml` and `DB_PORT` in `.env`.

### Film import fails

Set a valid `TMDB_API_KEY` in `.env` and restart the app.

### Drop the database (EF Core)

From the **`UmaFestHub`** folder, to remove the MySQL database only (containers keep running):

```bash
dotnet ef database drop --project src/UmaFestHub.Infrastructure --startup-project src/UmaFestHub.Web
```

EF will ask for confirmation. After dropping, recreate it with Step 4:

```bash
dotnet ef database update --project src/UmaFestHub.Infrastructure/UmaFestHub.Infrastructure.csproj --startup-project src/UmaFestHub.Web/UmaFestHub.Web.csproj
```

### Reset the database completely

```bash
docker compose down -v
docker compose up -d
dotnet ef database update --project src/UmaFestHub.Infrastructure/UmaFestHub.Infrastructure.csproj --startup-project src/UmaFestHub.Web/UmaFestHub.Web.csproj
```

---

## Sending this project to someone else

Include:

1. The full **`UmaFestHub`** folder (or a git clone).
2. The **`.env`** file (it is gitignored — copy it manually into the zip).
3. This **README**.

They do **not** need to commit secrets; each person can use their own `TMDB_API_KEY` if they prefer.

---

## Optional — run tests

```bash
dotnet test UmaFestHub.sln
```

---

## Architecture notes (short)

- **Stack:** ASP.NET Core MVC, EF Core, MySQL 8, SignalR (live notifications).
- **Background workers:** festival-ending and rental-expiry reminders (schedules in `appsettings.json`).
- **Personal lists:** Watchlist, Favorites, and Seen (`/PersonalList`).
