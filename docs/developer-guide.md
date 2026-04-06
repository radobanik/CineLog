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
| Seq (logs) | <http://localhost:5342> |
| pgAdmin | <http://localhost:5050> |
| MinIO console | <http://localhost:9001> (cinelog / cinelog123) |
| MinIO S3 API | `localhost:9000` |
| Elasticsearch | `localhost:9200` |
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

## Seq (Logs)

Seq collects logs from the API and the TmdbSync worker. Open <http://localhost:5342> and log in with `admin` / `admin`. Click **Events** to see the live log stream.

---

## MinIO (Blob Storage)

MinIO is an S3-compatible object store used for avatar uploads. It starts automatically with `docker compose up`.

- **S3 API**: `http://localhost:9000`
- **Browser console**: <http://localhost:9001> — login with `cinelog` / `cinelog123`

The `cinelog` bucket is created automatically on startup with public read access. Uploaded files are accessible at `http://localhost:9000/cinelog/<key>`.

The API connects to MinIO via the `BlobStorage` config section.

---

## TmdbSync

TmdbSync is a background worker that syncs movies, TV shows, and people from TMDb into the database. It starts automatically with `docker compose up` and runs every 24 hours.

It needs the TMDb API Read Access Token (the same value as `TMDB_API_KEY` in your `.env` file).

To run it locally from the IDE, set `DOTNET_ENVIRONMENT=Development` in your run configuration. This loads `appsettings.Development.json` which has smaller page limits so it finishes quickly.

---

## Local API Development

Start only the database, Redis and pgAdmin in the background (`-d` runs containers detached so they don't block your terminal):

```bash
docker compose up postgres redis pgadmin -d
```

Run the API:

```bash
dotnet run --project src/api/CineLog.Api/CineLog.Api.csproj
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

All commands below run from the repository root.

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

---

## API Client Generator

`tools/CineLog.ApiClientGenerator` is a console tool that generates typed C# clients from the running API's OpenAPI spec. It uses NSwag to generate the code and Roslyn to split the output into individual files.

### Output structure

```
GeneratedClients/
  Clients/
    IMoviesClient.cs
    MoviesClient.cs
    IReviewsClient.cs
    ReviewsClient.cs
    ...
  Models/
    MovieDetailResponse.cs
    ReviewResponse.cs
    ...
  Infrastructure/
    ApiException.cs
    FileResponse.cs
    ...
```

### Configuration

Default values live in `tools/CineLog.ApiClientGenerator/appsettings.json`:

```json
{
  "SwaggerUrl": "http://localhost:5098/swagger/v1/swagger.json",
  "OutputDir": "GeneratedClients",
  "Namespace": "CineLog.ApiClient"
}
```

All three settings can be overridden from the command line.

### Running

Start the API first (see [Local API Development](#local-api-development)), then run from the repository root:

```bash
# Use defaults from appsettings.json
dotnet run --project src/tools/CineLog.ApiClientGenerator/CineLog.ApiClientGenerator.csproj

# Override with positional args
dotnet run --project src/tools/CineLog.ApiClientGenerator/CineLog.ApiClientGenerator.csproj \
  -- http://localhost:5098/swagger/v1/swagger.json ./GeneratedClients CineLog.ApiClient

# Override with named args
dotnet run --project src/tools/CineLog.ApiClientGenerator/CineLog.ApiClientGenerator.csproj \
  -- --SwaggerUrl=http://localhost:5098/swagger/v1/swagger.json \
     --OutputDir=./GeneratedClients \
     --Namespace=CineLog.ApiClient
```

Command-line args take priority over `appsettings.json`. Relative `OutputDir` paths are resolved against the current working directory.
