using FlickFrenzyBot_Web_App.Entities;

namespace FlickFrenzyBot_Web_App.Abstractions
{
    public interface IRatingRepository
    {
        List<Rating> GetAllByMovieId(int movieId);
    }
}
