using System.Text.Json.Serialization;

namespace JMDB.Models
{
    public class TmdbSearchResponse
    {
        [JsonPropertyName("results")]
        public List<TmdbMovieResult> Results { get; set; } = new();
    }

    public class TmdbMovieResult
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("title")]
        public string Title { get; set; } = string.Empty;

        [JsonPropertyName("overview")]
        public string Overview { get; set; } = string.Empty;

        [JsonPropertyName("release_date")]
        public string ReleaseDate { get; set; } = string.Empty;

        [JsonPropertyName("vote_average")]
        public double VoteAverage { get; set; }

        [JsonPropertyName("poster_path")]
        public string? PosterPath { get; set; }

        public string? PosterUrl => PosterPath != null
            ? $"https://image.tmdb.org/t/p/w500{PosterPath}"
            : null;

        public int ReleaseYear => DateTime.TryParse(ReleaseDate, out var d) ? d.Year : 0;
    }

    public class TmdbMovieDetails : TmdbMovieResult
    {
        [JsonPropertyName("runtime")]
        public int Runtime { get; set; }

        [JsonPropertyName("genres")]
        public List<TmdbGenre> Genres { get; set; } = new();

        public string GenreNames => Genres.Any()
            ? string.Join(", ", Genres.Select(g => g.Name))
            : "Unknown";
    }

    public class TmdbGenre
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;
    }
}
