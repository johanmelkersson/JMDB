namespace JMDB.Models
{
    public class FavoriteMovie
    {
        public int Id { get; set; }

        public int MovieId { get; set; }
        public Movie? Movie { get; set; }

        public string UserId { get; set; } = string.Empty;
        public ApplicationUser? User { get; set; }
    }
}
