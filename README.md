# CineLog

A Letterboxd-inspired `.NET MAUI` mobile movie app backed by an `ASP.NET Core` REST API.

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
| Push notifications | Firebase Cloud Messaging |
| Search | Elasticsearch 8 |
| Logging | Seq |
| Blob storage | MinIO |
| External data | TMDb API |
| Data sync | CineLog.TmdbSync (background worker) |

## Local setup

Both files below are gitignored and must be added manually by each developer.

**Backend** — place the Firebase service account key at:
```
src/api/CineLog.Api/firebase-adminsdk.json
```
Download it from Firebase Console → Project Settings → Service accounts → Generate new private key.

**Mobile** — place the Google Services config at:
```
src/mobile/CineLog.Mobile/Platforms/Android/google-services.json
```
Download it from Firebase Console → Project Settings → Your Android app.

## Documentation

- [Architecture](docs/architecture.md)
- [Developer Guide](docs/developer-guide.md)
