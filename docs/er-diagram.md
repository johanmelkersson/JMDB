# ER Diagram – JMDB

```mermaid
erDiagram
    Movie {
        int Id PK
        string Title
        string Genre
        int ReleaseYear
        double Rating
        int Duration
        string PosterUrl
        string Description
        int TmdbId
    }

    ApplicationUser {
        string Id PK
        string UserName
        string Email
        datetime CreatedAt
    }

    Review {
        int Id PK
        string Content
        int Rating
        datetime CreatedAt
        int MovieId FK
        string UserId FK
    }

    FavoriteMovie {
        int Id PK
        int MovieId FK
        string UserId FK
    }

    IdentityRole {
        string Id PK
        string Name
    }

    Movie ||--o{ Review : "has"
    Movie ||--o{ FavoriteMovie : "saved in"
    ApplicationUser ||--o{ Review : "writes"
    ApplicationUser ||--o{ FavoriteMovie : "saves"
    ApplicationUser }o--o{ IdentityRole : "assigned"
```

## Tables

| Table | Description |
|---|---|
| Movies | The movie library — both manually created and imported from TMDB |
| AspNetUsers | Registered users (extends ASP.NET Identity) |
| Reviews | User reviews with a 1–10 rating, linked to movie and user |
| FavoriteMovies | Junction table — which users have favorited which movies |
| AspNetRoles | Roles: Admin, Member |
| AspNetUserRoles | Junction table — which roles each user has |
