# CineLog

A Letterboxd-inspired movie tracking app — .NET MAUI mobile client backed by an ASP.NET Core REST API.

## Overview

CineLog lets users log movies, write reviews, react to others' reviews, and follow friends. The mobile app is built with .NET MAUI (Android & iOS), and communicates with a self-hosted backend that integrates with TMDb for movie data.

## Tech Stack

| Component | Technology |
|---|---|
| Mobile app | .NET MAUI 10 |
| Backend API | ASP.NET Core 10 |
| Database | PostgreSQL 16 |
| Cache | Redis 7 |
| ORM | Entity Framework Core 10 |
| Auth | JWT Bearer |
| Real-time | SignalR |
| External data | TMDb API |

## Documentation

- [Architecture](docs/architecture.md)
- [API Reference](docs/api.md)
- [Developer Guide](docs/developer-guide.md)

## Quick Start

```bash
cp .env.example .env      
docker compose up --build
```

API (Docker): <http://localhost:5000/swagger>
