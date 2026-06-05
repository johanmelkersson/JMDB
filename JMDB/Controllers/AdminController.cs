using JMDB.Data;
using JMDB.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace JMDB.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public AdminController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Dashboard()
        {
            var topRated = await _context.Reviews
                .GroupBy(r => r.MovieId)
                .Select(g => new MovieStatRow
                {
                    Id = g.Key,
                    AverageRating = g.Average(r => r.Rating),
                    ReviewCount = g.Count()
                })
                .OrderByDescending(x => x.AverageRating)
                .Take(5)
                .ToListAsync();

            // Fill in movie details
            foreach (var row in topRated)
            {
                var movie = await _context.Movies.FindAsync(row.Id);
                if (movie != null) { row.Title = movie.Title; row.PosterUrl = movie.PosterUrl; }
            }

            var mostReviewed = await _context.Reviews
                .GroupBy(r => r.MovieId)
                .Select(g => new MovieStatRow
                {
                    Id = g.Key,
                    ReviewCount = g.Count(),
                    AverageRating = g.Average(r => r.Rating)
                })
                .OrderByDescending(x => x.ReviewCount)
                .Take(5)
                .ToListAsync();

            foreach (var row in mostReviewed)
            {
                var movie = await _context.Movies.FindAsync(row.Id);
                if (movie != null) { row.Title = movie.Title; row.PosterUrl = movie.PosterUrl; }
            }

            var mostActive = await _context.Reviews
                .GroupBy(r => r.UserId)
                .Select(g => new { UserId = g.Key, ReviewCount = g.Count() })
                .OrderByDescending(x => x.ReviewCount)
                .Take(5)
                .ToListAsync();

            var userStats = new List<UserStatRow>();
            foreach (var u in mostActive)
            {
                var user = await _context.Users.FindAsync(u.UserId);
                var favCount = await _context.FavoriteMovies.CountAsync(f => f.UserId == u.UserId);
                userStats.Add(new UserStatRow
                {
                    UserName = user?.UserName ?? "Unknown",
                    ReviewCount = u.ReviewCount,
                    FavoriteCount = favCount
                });
            }

            var viewModel = new DashboardViewModel
            {
                TotalMovies = await _context.Movies.CountAsync(),
                TotalUsers = await _context.Users.CountAsync(),
                TotalReviews = await _context.Reviews.CountAsync(),
                TotalFavorites = await _context.FavoriteMovies.CountAsync(),
                TopRatedMovies = topRated,
                MostReviewedMovies = mostReviewed,
                MostActiveUsers = userStats,
                RecentlyImported = await _context.Movies
                    .Where(m => m.TmdbId != null)
                    .OrderByDescending(m => m.Id)
                    .Take(6)
                    .ToListAsync()
            };

            return View(viewModel);
        }

        // GET: /Admin/Users
        public async Task<IActionResult> Users()
        {
            var users = await _context.Users.ToListAsync();
            var model = new List<(ApplicationUser User, IList<string> Roles, int Reviews)>();

            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);
                var reviewCount = await _context.Reviews.CountAsync(r => r.UserId == user.Id);
                model.Add((user, roles, reviewCount));
            }

            return View(model);
        }

        // POST: /Admin/ToggleRole
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleRole(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return NotFound();

            // Don't allow demoting yourself
            if (user.UserName == User.Identity!.Name)
                return RedirectToAction(nameof(Users));

            if (await _userManager.IsInRoleAsync(user, "Admin"))
            {
                await _userManager.RemoveFromRoleAsync(user, "Admin");
                await _userManager.AddToRoleAsync(user, "Member");
            }
            else
            {
                await _userManager.RemoveFromRoleAsync(user, "Member");
                await _userManager.AddToRoleAsync(user, "Admin");
            }

            return RedirectToAction(nameof(Users));
        }
    }
}
