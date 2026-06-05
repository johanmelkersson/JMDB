# User Roles – JMDB

JMDB uses three roles managed by ASP.NET Identity.

```mermaid
flowchart TD
    Guest["👤 Guest\n(not logged in)"]
    Member["👥 Member\n(registered user)"]
    Admin["⚙️ Admin"]

    Guest -->|Register| Member
    Member -->|Promoted by Admin| Admin

    Guest --- G1[Browse movies]
    Guest --- G2[Search movies]
    Guest --- G3[Read reviews]
    Guest --- G4[Browse TMDB Discover]

    Member --- M1[All Guest permissions]
    Member --- M2[Write reviews & ratings]
    Member --- M3[Add / remove favorites]
    Member --- M4[Delete own reviews]

    Admin --- A1[All Member permissions]
    Admin --- A2[Create / Edit / Delete movies]
    Admin --- A3[Import movies from TMDB]
    Admin --- A4[Delete any review]
    Admin --- A5[Access admin dashboard]
    Admin --- A6[Manage users & roles]
```

## Permission Matrix

| Feature | Guest | Member | Admin |
|---|:---:|:---:|:---:|
| Browse & search movies | ✅ | ✅ | ✅ |
| Read reviews | ✅ | ✅ | ✅ |
| Browse TMDB Discover | ✅ | ✅ | ✅ |
| Register / Login | ✅ | — | — |
| Write reviews & ratings | ❌ | ✅ | ✅ |
| Add to favorites | ❌ | ✅ | ✅ |
| Delete own reviews | ❌ | ✅ | ✅ |
| Create / Edit / Delete movies | ❌ | ❌ | ✅ |
| Import from TMDB | ❌ | ❌ | ✅ |
| Delete any review | ❌ | ❌ | ✅ |
| Admin dashboard | ❌ | ❌ | ✅ |
| Manage users & roles | ❌ | ❌ | ✅ |
