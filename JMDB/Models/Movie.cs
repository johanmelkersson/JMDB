namespace JMDB.Models
{
    public class Movie
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Genre { get; set; } = string.Empty;
        public int ReleaseYear { get; set; }
        public double Rating { get; set; }
        public int Duration { get; set; }
        public string? PosterUrl { get; set; }
        public string? Description { get; set; }
    }
}