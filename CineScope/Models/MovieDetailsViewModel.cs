namespace CineScope.Models
{
    public class MovieDetailsViewModel
    {
        public Movie Movie { get; set; } = null!;
        public List<Review> Reviews { get; set; } = new();
        public double AverageRating { get; set; }
        public bool IsFavorite { get; set; }
        public bool UserHasReviewed { get; set; }

        // Form fields for new review
        public string NewReviewContent { get; set; } = string.Empty;
        public int NewReviewRating { get; set; } = 7;
    }
}
