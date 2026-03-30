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
- **TMDb API** — external source for movie data

## Notes

- Features are organized as vertical slices inside `Application/Features/`. Each feature has its own handler, command/query, and validator in one place.
- The API applies pending database migrations automatically on startup.
- SignalR pushes a notification to a user when their review receives a reaction.
