namespace JMDB.Models
{
    public class DashboardViewModel
    {
        public int TotalMovies { get; set; }
        public int TotalUsers { get; set; }
        public int TotalReviews { get; set; }
        public int TotalFavorites { get; set; }

        public List<MovieStatRow> TopRatedMovies { get; set; } = new();
        public List<MovieStatRow> MostReviewedMovies { get; set; } = new();
        public List<UserStatRow> MostActiveUsers { get; set; } = new();
        public List<Movie> RecentlyImported { get; set; } = new();
    }

    public class MovieStatRow
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? PosterUrl { get; set; }
        public double AverageRating { get; set; }
        public int ReviewCount { get; set; }
    }

    public class UserStatRow
    {
        public string UserName { get; set; } = string.Empty;
        public int ReviewCount { get; set; }
        public int FavoriteCount { get; set; }
    }
}
