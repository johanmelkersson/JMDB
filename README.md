# JMDB – Movie Discovery Platform

A full-stack movie discovery platform built with ASP.NET Core MVC as a school project. Users can browse, rate and review movies, while admins can manage the movie library with data fetched directly from the TMDB API.

---

## Tech Stack

| Layer | Technology |
|---|---|
| Backend | ASP.NET Core 10 MVC |
| Database | SQL Server + Entity Framework Core (Code First) |
| Authentication | ASP.NET Identity with role-based authorization |
| Frontend | Razor Views + Bootstrap 5 |
| External API | TMDB API |
| Real-Time | SignalR |

---

## Features

### Movie Management
- Full CRUD for movies (Admin only)
- Search and filter by genre
- Import movies directly from TMDB with poster, metadata and description auto-filled

### Authentication & Roles
| Role | Permissions |
|---|---|
| Guest | Browse movies, search, read reviews |
| Member | Everything above + write reviews, rate movies, add favorites |
| Admin | Everything above + manage movies, manage users, moderate reviews, access dashboard |

### Reviews & Ratings
- Members can rate movies (1–10) and write a review
- Average rating calculated from all user reviews
- Real-time review updates via SignalR — reviews appear live without page reload
- Real-time active user counter showing how many members are currently online

### TMDB Integration
- Browse trending movies from TMDB
- Search the TMDB database
- Import any movie to the local library with one click (Admin)
- Imported movies are tracked to prevent duplicates

### Admin Dashboard
- Overview stats: total movies, users, reviews, favorites
- Top rated and most reviewed movies
- Most active users
- Recently imported movies
- User management: view all users and promote/demote roles

---

## Getting Started

### Prerequisites
- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- SQL Server or SQL Server LocalDB
- A free [TMDB API key](https://www.themoviedb.org/settings/api)

### Setup

1. **Clone the repository**
   ```bash
   git clone <repo-url>
   cd CineScope/JMDB
   ```

2. **Add your TMDB API key using .NET User Secrets**
   ```bash
   dotnet user-secrets init
   dotnet user-secrets set "Tmdb:ApiKey" "your-api-key-here"
   ```

3. **Update the connection string** in `appsettings.json` if needed
   ```json
   "ConnectionStrings": {
     "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=JMDB;Trusted_Connection=True;"
   }
   ```

4. **Apply migrations**
   ```bash
   dotnet ef database update
   ```

5. **Run the application**
   ```bash
   dotnet run
   ```

### Default Admin Account
An admin account is seeded automatically on first run:

| Field | Value |
|---|---|
| Username | `admin` |
| Password | `Admin123!` |

> Change the password after first login.

---

## Project Structure

```
JMDB/
├── Controllers/        # MVC controllers
├── Data/               # ApplicationDbContext
├── Hubs/               # SignalR hub
├── Migrations/         # EF Core migrations
├── Models/             # Domain models and view models
├── Services/           # TmdbService, UserTracker
├── Views/              # Razor views
└── wwwroot/            # Static assets (CSS, JS)
```

---

## School Project — Level Completion

| Level | Description | Status |
|---|---|---|
| 1 | Foundation – CRUD, EF Core, Movie model | ✅ |
| 2 | Modern UI – Netflix-style, dark mode, hero banner | ✅ |
| 3 | Authentication & Roles – Identity, Guest/Member/Admin | ✅ |
| 4 | Reviews & Ratings – rate, review, favorites | ✅ |
| 5 | External API – TMDB integration | ✅ |
| 6 | Dashboard Analytics – admin stats | ✅ |
| 7 | Real-Time – SignalR live reviews, active users | ✅ |
| 8 | Deployment – Azure/Docker (optional) | ⏸️ |
