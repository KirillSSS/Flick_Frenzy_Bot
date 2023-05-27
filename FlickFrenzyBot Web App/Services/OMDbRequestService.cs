using System.Reflection;
using System.Text.Json;
using FlickFrenzyBot_Web_App.Abstractions;
using FlickFrenzyBot_Web_App.Database;
using FlickFrenzyBot_Web_App.Database.Repositories;
using FlickFrenzyBot_Web_App.Entities;

namespace FlickFrenzyBot_Web_App.Services
{
    public class OMDbRequestService : IOMDbRequestService
    {
        private readonly HttpClient _httpClient;
        private readonly string? _apiUrl;
        private readonly string? _apiKey;
        private readonly BotDbContext _dbContext; 
        private readonly IMovieRepository _movieRepository; 

        public OMDbRequestService(HttpClient httpClient, IConfiguration configuration, BotDbContext dbContext)
        {
            _httpClient = httpClient;
            _apiUrl = configuration["OMDbApi:ApiUrl"];
            _apiKey = configuration["OMDbApi:ApiKey"];
            _dbContext = dbContext;
            _movieRepository = new MovieRepository(_dbContext);
        }

        public async Task<(bool isCorrect, Movie? outMovie)> GetMovieAsync(string name)
        {
            var movie = _movieRepository.GetByTitle(name);

            if (movie is not null)
                return (true, movie);

            var request = new HttpRequestMessage(HttpMethod.Get, $"{_apiUrl}?apikey={_apiKey}&t={name}");

            var response = await _httpClient.SendAsync(request);
            var responseContent = await response.Content.ReadAsStringAsync();

            movie = JsonSerializer.Deserialize<Movie>(responseContent);

            if (movie is null || movie.Response == "False")
                return (false, null);

            movie = _movieRepository.GetByTitle(movie.Title);

            if (movie is not null)
                return (true, movie);

            _movieRepository.Create(movie);
            return (true, movie);
        }
    }
}
