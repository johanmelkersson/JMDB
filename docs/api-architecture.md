# API Architecture – JMDB

## Overview

JMDB integrates with the [TMDB API](https://developer.themoviedb.org/) (The Movie Database) to fetch movie data, posters and trending information.

```mermaid
flowchart LR
    Browser["🌐 Browser"]
    App["JMDB\nASP.NET Core MVC"]
    TMDB["TMDB API\napi.themoviedb.org"]
    DB["SQL Server\nJMDB Database"]

    Browser -->|HTTP Request| App
    App -->|HttpClient GET| TMDB
    TMDB -->|JSON Response| App
    App -->|EF Core| DB
    DB -->|Query Result| App
    App -->|Razor View| Browser
```

## TMDB Endpoints Used

| Endpoint | Description |
|---|---|
| `GET /trending/movie/week` | Fetches trending movies for the Discover page |
| `GET /search/movie?query=...` | Searches TMDB by movie title |
| `GET /movie/{id}` | Fetches full movie details for import (includes runtime and genres) |

**Base URL:** `https://api.themoviedb.org/3/`  
**Poster images:** `https://image.tmdb.org/t/p/w500/{poster_path}`

## Import Flow

```mermaid
sequenceDiagram
    actor Admin
    participant App as JMDB App
    participant TMDB as TMDB API
    participant DB as Database

    Admin->>App: Opens Discover page
    App->>TMDB: GET /trending/movie/week
    TMDB-->>App: JSON list of movies
    App-->>Admin: Shows movie cards

    Admin->>App: Clicks "+ Import" on a card
    App->>TMDB: GET /movie/{tmdbId}
    TMDB-->>App: Full movie details
    App->>DB: INSERT into Movies
    App-->>Admin: Redirects to movie detail page
```

## Security

- The TMDB API key is stored using **.NET User Secrets** in development and must be set via environment variables in production
- The key is never committed to source control
- Import is restricted to **Admin** role only
