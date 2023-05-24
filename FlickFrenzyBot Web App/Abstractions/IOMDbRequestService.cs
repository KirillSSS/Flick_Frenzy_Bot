using FlickFrenzyBot_Web_App.Entities;

namespace FlickFrenzyBot_Web_App.Abstractions
{
    public interface IOMDbRequestService
    {
        Task<(bool isCorrect, Movie? outMovie)> GetMovieAsync(string name);
    }
}
