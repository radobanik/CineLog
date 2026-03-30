# Developer Guide

## Requirements

- [Docker Desktop](https://www.docker.com/products/docker-desktop/)
- [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)
- TMDb API key — free at <https://www.themoviedb.org/settings/api>

---

## Running with Docker

Copy the env file and add your TMDb key:

```bash
cp .env.example .env
# open .env and set TMDB_API_KEY
```

Make sure Docker Desktop is open and running before any `docker compose` command.

Start everything:

```bash
# with logs visible in the terminal
docker compose up --build

# or in the background (detached)
docker compose up --build -d
```

| Service | URL |
|---|---|
| API + Swagger | <http://localhost:5000/swagger> |
| pgAdmin | <http://localhost:5050> |
| PostgreSQL | `localhost:5432` |
| Redis | `localhost:6379` |

To stop (keeps data):

```bash
docker compose down
```

To stop and wipe the database:

```bash
docker compose down -v
```

---

## Local API Development

Start only the database, Redis and pgAdmin in the background (`-d` runs containers detached so they don't block your terminal):

```bash
docker compose up postgres redis pgadmin -d
```

Run the API:

```bash
cd src/CineLog
dotnet run --project src/CineLog.Api/CineLog.Api.csproj
```

Swagger UI: <http://localhost:5098/swagger>

---

## pgAdmin

pgAdmin is available at <http://localhost:5050> when Docker is running.

Login with:
- **Email**: `admin@cinelog.dev`
- **Password**: `admin`

To connect to the database after logging in:

1. In the left panel, right-click **Servers** → **Register** → **Server**
2. **General tab** — set Name to `cinelog`
3. **Connection tab** — fill in:
   - Host: `postgres`
   - Port: `5432`
   - Username: `postgres`
   - Password: `postgres`
4. Click **Save**

> Use `postgres` as the host, not `localhost`. Both pgAdmin and the database run inside Docker and communicate by service name.

Tables are located in the object explorer under: **Servers → cinelog → Databases → cinelog → Schemas → public → Tables**

---

## Seed Data

The API seeds the database automatically on startup. The following accounts are created if they don't already exist:

| Username | Email | Password | Role |
|---|---|---|---|
| admin_alice | alice@cinelog.dev | Admin1234! | Admin |
| admin_bob | bob@cinelog.dev | Admin1234! | Admin |
| user_carol | carol@cinelog.dev | User1234! | User |
| user_dave | dave@cinelog.dev | User1234! | User |

---

## Migrations

Migrations are in `src/CineLog.Infrastructure/Data/Migrations/`. The API applies them automatically on startup, so you only need these commands when making schema changes.

Install the EF tools if you haven't already:

```bash
dotnet tool install --global dotnet-ef
```

All commands below run from the `src/CineLog/` directory.

**Add a migration** after changing a domain entity:

```bash
dotnet ef migrations add <Name> \
  --project src/CineLog.Infrastructure/CineLog.Infrastructure.csproj \
  --startup-project src/CineLog.Api/CineLog.Api.csproj
```

**Apply migrations manually** (normally not needed — the API does this on startup):

```bash
dotnet ef database update \
  --project src/CineLog.Infrastructure/CineLog.Infrastructure.csproj \
  --startup-project src/CineLog.Api/CineLog.Api.csproj
```

**Remove the last migration** (only if it hasn't been applied yet):

```bash
dotnet ef migrations remove \
  --project src/CineLog.Infrastructure/CineLog.Infrastructure.csproj \
  --startup-project src/CineLog.Api/CineLog.Api.csproj
```
