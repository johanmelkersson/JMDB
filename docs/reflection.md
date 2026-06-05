# Reflection Report – JMDB

## Project Overview

JMDB is a movie discovery platform built as a school project using ASP.NET Core MVC. The goal was to build a full-stack web application from scratch, progressing through multiple levels of increasing complexity — from basic CRUD to real-time features and external API integration.

The application allows users to browse and discover movies, write reviews and ratings, and save favorites. Admins can manage the movie library by importing data directly from the TMDB API, and moderate user content through a dedicated dashboard. Real-time features were implemented using SignalR, enabling live review updates and an active user counter.

---

## What I Learned

### ASP.NET Core MVC

Working with ASP.NET Core MVC gave me a solid understanding of how the pattern works in practice. The separation between Controllers, Models and Views made the codebase easy to navigate — the controller handles the request, fetches or modifies data, and passes exactly what the view needs.

One thing that stood out was the use of ViewModels. Rather than passing a raw model directly to a view, I learned to create dedicated ViewModels like `MovieDetailsViewModel` that bundle together all the data a view needs in one place. This made the views cleaner and avoided overloading the domain models with view-specific logic.

I also got comfortable with dependency injection — injecting services like `DbContext`, `UserManager` and `IHubContext` through constructors rather than instantiating them manually. Razor Tag Helpers (`asp-for`, `asp-controller`, `asp-action`) were another practical improvement over hardcoded URLs, making the views more maintainable. Finally, working with Razor Sections allowed me to break out of the standard layout when needed, such as rendering the full-width hero banner outside the container.

### Entity Framework Core

Having used EF Core in a previous console project, this was a chance to apply it in a web context with more complex relationships. The Code First approach — defining models in C# and letting EF Core generate the database — continued to feel natural, and migrations made schema changes straightforward. Every time a model changed, such as adding the `TmdbId` field to `Movie`, it was just a matter of running `Add-Migration` and `Update-Database`.

The relational side was more challenging this time. Models like `Review` and `FavoriteMovie` required foreign keys to both `Movie` and `ApplicationUser`, and I learned how EF Core handles these relationships through navigation properties and `.Include()` for eager loading. Writing LINQ queries for the dashboard — grouping reviews by movie, calculating averages, ordering results — showed how much you can express without writing a single line of SQL. One notable change was updating `ApplicationDbContext` to inherit from `IdentityDbContext` when adding ASP.NET Identity, which seamlessly merged the Identity tables into the same database.

### ASP.NET Identity & Authorization

ASP.NET Identity was one of the more involved parts of the project. Setting it up required installing the right package, creating a custom `ApplicationUser` class extending `IdentityUser`, updating `ApplicationDbContext` to inherit from `IdentityDbContext`, and registering all the necessary services in `Program.cs`. Once in place though, it handled a lot automatically — password hashing, login sessions and cookie management included.

Role-based authorization was straightforward once the foundation was set. Roles are seeded on startup using `RoleManager`, and locking down specific actions is as simple as adding `[Authorize(Roles = "Admin")]` to a controller method. Working with `UserManager` and `SignInManager` in the `AccountController` gave me a clear picture of how user creation, login and logout work under the hood. It was also useful to learn how Identity uses cookie-based authentication and how to configure redirect paths when a user tries to access something they are not allowed to.

### External API Integration

Integrating the TMDB API introduced me to working with external HTTP services in a structured way. Rather than creating an `HttpClient` manually, I registered a dedicated `TmdbService` using `AddHttpClient<TmdbService>()` in `Program.cs`, which lets the framework manage the client's lifetime and connection pooling correctly.

Deserializing the API responses required mapping TMDB's snake_case JSON fields to C# properties using `[JsonPropertyName]` attributes from `System.Text.Json`. One thing I noticed early on was that a search result doesn't include everything — runtime and genre names require a separate call to the `/movie/{id}` endpoint. This meant the import flow involved two API calls rather than one.

Handling the API key securely was also a valuable lesson. Using .NET User Secrets kept the key out of the codebase entirely, so it could never accidentally be committed to git. On the data side, storing `TmdbId` on the `Movie` model made it possible to check whether a movie had already been imported, preventing duplicates and enabling the toggle button on the Discover page.

### SignalR

SignalR was the most conceptually different technology in the project. Unlike regular HTTP where the client always initiates a request and waits for a response, SignalR keeps a persistent connection open so the server can push data to clients at any time. This was what made live review updates possible — when a review is posted, the server broadcasts it to everyone currently viewing that movie, without any of them having to refresh.

The central concept is the Hub. `ReviewHub` acts as the connection point on the server, and clients connect to it via WebSocket. Using groups meant that only users on the same movie's detail page receive its review updates — achieved by having the client call `JoinMovieGroup` on connect. Sending messages from a controller rather than directly from the hub required injecting `IHubContext<ReviewHub>`, which was a useful pattern to learn.

On the client side, the SignalR JavaScript library handles the connection and listens for named events with `.on("EventName", ...)`. Overriding `OnConnectedAsync` and `OnDisconnectedAsync` in the hub made it possible to track active logged-in users in real time, using a singleton `UserTracker` service that counts unique user IDs rather than raw connections.

---

## Challenges

The most complex part of the SignalR implementation was managing UI state in the browser without a page reload. Getting the review form, the "already reviewed" message and the review count to all update correctly across multiple tabs required that every relevant element was always present in the DOM and toggled with JavaScript rather than conditionally rendered server-side. The live delete functionality added another layer — the `ReviewDeleted` event needed to carry the author's username so the correct tab could restore the form.

Tracking active users went through several iterations. A simple integer counter was the obvious starting point, but it counted every tab as a separate user. Switching to a `HashSet` of user IDs solved the uniqueness problem but caused the user to disappear when closing one of two open tabs, since the single connection disconnecting removed them entirely. The final solution used a `Dictionary<string, int>` to track connection counts per user, only removing them when their last connection closed.

Writing the LINQ queries for the admin dashboard was also more involved than expected. Grouping reviews by movie, calculating averages and joining back to the movie table to get titles required thinking carefully about what EF Core translates to SQL and what needs to happen in memory.

Setting up ASP.NET Identity required getting several things right simultaneously — the NuGet package, changing `ApplicationDbContext` to inherit from `IdentityDbContext`, registering services and configuring cookie redirects in `Program.cs`. Missing any one of these caused the application to fail in ways that weren't always obvious to diagnose.

---

## What I Would Do Differently

Planning the data model more carefully upfront would have saved some work. The `TmdbId` field on `Movie` was added as an afterthought when it became clear it was needed to track imported movies — a bit of upfront thinking about how the TMDB integration would work could have included it from the start and avoided an extra migration.

The `MoviesController` grew quite large over time. It ended up handling not just movie CRUD but also reviews, favorites and SignalR notifications. Splitting these into separate controllers — a `ReviewsController` and keeping `MoviesController` focused on movies — would have made the code easier to navigate and maintain.

I would also set up input validation more thoroughly from the beginning. The forms work, but the Movie model lacks data annotations like `[Required]`, `[Range]` and `[StringLength]` that would give better feedback and protect the database from bad data.

Finally, using .NET User Secrets from day one rather than waiting until the API key was needed would be a good habit to establish earlier. Any value that could become sensitive — connection strings, passwords — is better handled that way from the start.

---

## Summary

This project covered a lot of ground — from basic CRUD and database migrations to external API integration, role-based authorization and real-time features with SignalR. Each level built on the previous one, which made the progression feel natural even when the concepts got more complex.

The part I am most satisfied with is the SignalR implementation. Getting live review updates, form state synchronization across tabs and accurate active user tracking all working correctly required careful thinking about both the server and client side. It is the feature that feels most unlike a typical school project.

Overall, JMDB went from a simple movie list to a functioning platform with a real feature set. The technologies used — ASP.NET Core, Entity Framework Core, Identity and SignalR — are all relevant in professional .NET development, which makes the project feel like useful preparation beyond just meeting the assignment requirements.
