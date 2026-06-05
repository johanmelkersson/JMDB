using System.Text.Json;
using JMDB.Models;

namespace JMDB.Services
{
    public class TmdbService
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey;

        public TmdbService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _apiKey = configuration["Tmdb:ApiKey"]
                ?? throw new InvalidOperationException("TMDB API key not configured. Run: dotnet user-secrets set \"Tmdb:ApiKey\" \"your-key\"");
            _httpClient.BaseAddress = new Uri("https://api.themoviedb.org/3/");
        }

        public async Task<List<TmdbMovieResult>> SearchAsync(string query)
        {
            var url = $"search/movie?api_key={_apiKey}&query={Uri.EscapeDataString(query)}&language=en-US";
            var json = await _httpClient.GetStringAsync(url);
            var result = JsonSerializer.Deserialize<TmdbSearchResponse>(json);
            return result?.Results ?? new();
        }

        public async Task<List<TmdbMovieResult>> GetTrendingAsync()
        {
            var url = $"trending/movie/week?api_key={_apiKey}&language=en-US";
            var json = await _httpClient.GetStringAsync(url);
            var result = JsonSerializer.Deserialize<TmdbSearchResponse>(json);
            return result?.Results.Take(12).ToList() ?? new();
        }

        public async Task<TmdbMovieDetails?> GetDetailsAsync(int tmdbId)
        {
            var url = $"movie/{tmdbId}?api_key={_apiKey}&language=en-US";
            var response = await _httpClient.GetAsync(url);
            if (!response.IsSuccessStatusCode) return null;
            var json = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<TmdbMovieDetails>(json);
        }
    }
}
