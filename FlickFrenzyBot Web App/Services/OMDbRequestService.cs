using System.Reflection;
using System.Text.Json;
using FlickFrenzyBot_Web_App.Abstractions;
using FlickFrenzyBot_Web_App.Database;
using FlickFrenzyBot_Web_App.Database.Repositories;
using FlickFrenzyBot_Web_App.Entities;

namespace FlickFrenzyBot_Web_App.Services
{
    public class OMDbRequestService : IRequestService
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiUrl;
        private readonly string _apiKey;
        private readonly string _badUrl;
        private readonly BotDbContext _dbContext; 
        private readonly IMovieRepository _movieRepository; 

        public OMDbRequestService(HttpClient httpClient, IConfiguration configuration, BotDbContext dbContext)
        {
            _httpClient = httpClient;
            _apiUrl = configuration["OMDbApi:ApiUrl"];
            _apiKey = configuration["OMDbApi:ApiKey"];
            _badUrl = configuration["OMDbApi:BadUrl"];
            _dbContext = dbContext;
            _movieRepository = new MovieRepository(_dbContext);
        }

        public async Task<(string PosterUrl, string Info)> GetResponseAsync(string message)
        {
            Console.WriteLine(message);

            var request = new HttpRequestMessage(HttpMethod.Get, $"{_apiUrl}?apikey={_apiKey}&t={message}");

            var response = await _httpClient.SendAsync(request);
            var responseContent = await response.Content.ReadAsStringAsync();

            var movie = JsonSerializer.Deserialize<Movie>(responseContent);
            var output = "";

            if (movie is null || movie.Response == "False")
                return (_badUrl, "Sorry, there is no such film");

            var movieRepo = new MovieRepository(_dbContext);

            if (movieRepo.GetByTitle(movie.Title) is null)
                movieRepo.Create(movie);

            var rating = "";

            foreach (var property in movie.Ratings)
            {
                rating += $"    {property.Source}: {property.Value}\n";
            }

            output = $"{movie.Title}: \n{rating}";

            return (movie.Poster, output);
        }
    }
}
