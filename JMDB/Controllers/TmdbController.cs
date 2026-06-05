using JMDB.Data;
using JMDB.Models;
using JMDB.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace JMDB.Controllers
{
    public class TmdbController : Controller
    {
        private readonly TmdbService _tmdb;
        private readonly ApplicationDbContext _context;

        public TmdbController(TmdbService tmdb, ApplicationDbContext context)
        {
            _tmdb = tmdb;
            _context = context;
        }

        // GET: /Tmdb/Search
        public async Task<IActionResult> Search(string? query)
        {
            List<TmdbMovieResult> results;

            if (string.IsNullOrWhiteSpace(query))
            {
                results = await _tmdb.GetTrendingAsync();
                ViewData["Mode"] = "Trending";
            }
            else
            {
                results = await _tmdb.SearchAsync(query);
                ViewData["Query"] = query;
                ViewData["Mode"] = "Search";
            }

            // Fetch which TMDB IDs are already imported
            var tmdbIds = results.Select(r => r.Id).ToList();
            var importedIds = await _context.Movies
                .Where(m => m.TmdbId != null && tmdbIds.Contains(m.TmdbId!.Value))
                .Select(m => m.TmdbId!.Value)
                .ToHashSetAsync();

            ViewData["ImportedIds"] = importedIds;
            return View(results);
        }

        // POST: /Tmdb/Import
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Import(int tmdbId)
        {
            // Guard: already imported?
            if (await _context.Movies.AnyAsync(m => m.TmdbId == tmdbId))
                return RedirectToAction(nameof(Search));

            var details = await _tmdb.GetDetailsAsync(tmdbId);
            if (details == null) return NotFound();

            var movie = new Movie
            {
                Title = details.Title,
                Description = details.Overview,
                ReleaseYear = details.ReleaseYear,
                Rating = Math.Round(details.VoteAverage, 1),
                Duration = details.Runtime,
                Genre = details.GenreNames,
                PosterUrl = details.PosterUrl,
                TmdbId = tmdbId
            };

            _context.Movies.Add(movie);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Search));
        }

        // POST: /Tmdb/Remove
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Remove(int tmdbId)
        {
            var movie = await _context.Movies.FirstOrDefaultAsync(m => m.TmdbId == tmdbId);
            if (movie != null)
            {
                _context.Movies.Remove(movie);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Search));
        }
    }
}
