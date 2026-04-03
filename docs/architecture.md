# Architecture

CineLog has two parts: a .NET MAUI mobile app (Android and iOS) and an ASP.NET Core 10 backend. The mobile app talks to the backend over HTTP and WebSocket.

## Backend projects

- `CineLog.Api` — controllers, middleware, SignalR hub, app entry point
- `CineLog.Application` — business logic, MediatR handlers, validation
- `CineLog.Domain` — entities, value objects, domain exceptions
- `CineLog.Infrastructure` — database access, repositories, JWT, TMDb client, Redis cache
- `CineLog.Contracts` — shared request/response DTOs

## Infrastructure

- **PostgreSQL** — main database
- **Redis** — query cache (EF Core second-level cache)
- **Elasticsearch** — full-text search for movies and people. Two indices: `cinelog-movies` and `cinelog-people`. The API creates and populates them on startup.
- **Seq** — centralized log viewer. Both the API and TmdbSync send logs here. UI at `http://localhost:5342`.
- **TMDb API** — external source for movie data

## TmdbSync

`CineLog.TmdbSync` is a background worker that runs alongside the API. It fetches movies, TV shows, and people from TMDb and writes them to the shared PostgreSQL database. It runs on a schedule (default: every 24 hours) and tracks its progress so it can resume if interrupted.

## Notes

- Features are organized as vertical slices inside `Application/Features/`. Each feature has its own handler, command/query, and validator in one place.
- The API applies pending database migrations automatically on startup.
- SignalR pushes a notification to a user when their review receives a reaction.
