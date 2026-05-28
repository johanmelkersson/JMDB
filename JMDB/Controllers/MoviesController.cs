using JMDB.Data;
using JMDB.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace JMDB.Controllers
{
    public class MoviesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public MoviesController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Movies
        public async Task<IActionResult> Index(string? searchString)
        {
            var movies = _context.Movies.AsQueryable();

            if (!string.IsNullOrEmpty(searchString))
            {
                movies = movies.Where(m => m.Title.Contains(searchString));
                ViewData["SearchString"] = searchString;
            }

            return View(await movies.ToListAsync());
        }

        // GET: Movies/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var movie = await _context.Movies.FirstOrDefaultAsync(m => m.Id == id);
            if (movie == null) return NotFound();

            var reviews = await _context.Reviews
                .Include(r => r.User)
                .Where(r => r.MovieId == id)
                .OrderByDescending(r => r.CreatedAt)
                .ToListAsync();

            var userId = _userManager.GetUserId(User);
            var isFavorite = userId != null && await _context.FavoriteMovies.AnyAsync(f => f.MovieId == id && f.UserId == userId);
            var userHasReviewed = userId != null && reviews.Any(r => r.UserId == userId);
            var averageRating = reviews.Any() ? Math.Round(reviews.Average(r => r.Rating), 1) : 0;

            var viewModel = new MovieDetailsViewModel
            {
                Movie = movie,
                Reviews = reviews,
                AverageRating = averageRating,
                IsFavorite = isFavorite,
                UserHasReviewed = userHasReviewed
            };

            return View(viewModel);
        }

        // POST: Movies/AddReview
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> AddReview(int movieId, string content, int rating)
        {
            var userId = _userManager.GetUserId(User);
            if (userId == null) return Unauthorized();

            var alreadyReviewed = await _context.Reviews.AnyAsync(r => r.MovieId == movieId && r.UserId == userId);
            if (!alreadyReviewed)
            {
                _context.Reviews.Add(new Review
                {
                    MovieId = movieId,
                    UserId = userId,
                    Content = content,
                    Rating = Math.Clamp(rating, 1, 10)
                });
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Details), new { id = movieId });
        }

        // POST: Movies/ToggleFavorite
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> ToggleFavorite(int movieId)
        {
            var userId = _userManager.GetUserId(User);
            if (userId == null) return Unauthorized();

            var existing = await _context.FavoriteMovies.FirstOrDefaultAsync(f => f.MovieId == movieId && f.UserId == userId);

            if (existing != null)
                _context.FavoriteMovies.Remove(existing);
            else
                _context.FavoriteMovies.Add(new FavoriteMovie { MovieId = movieId, UserId = userId });

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Details), new { id = movieId });
        }

        // GET: Movies/Create
        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Movies/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create(Movie movie)
        {
            if (ModelState.IsValid)
            {
                _context.Add(movie);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(movie);
        }

        // GET: Movies/Edit/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var movie = await _context.Movies.FindAsync(id);
            if (movie == null) return NotFound();

            return View(movie);
        }

        // POST: Movies/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id, Movie movie)
        {
            if (id != movie.Id) return NotFound();

            if (ModelState.IsValid)
            {
                _context.Update(movie);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(movie);
        }

        // GET: Movies/Delete/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var movie = await _context.Movies.FirstOrDefaultAsync(m => m.Id == id);
            if (movie == null) return NotFound();

            return View(movie);
        }

        // POST: Movies/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var movie = await _context.Movies.FindAsync(id);
            if (movie != null)
            {
                _context.Movies.Remove(movie);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
