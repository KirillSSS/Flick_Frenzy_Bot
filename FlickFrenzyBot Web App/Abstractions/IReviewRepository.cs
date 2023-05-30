using FlickFrenzyBot_Web_App.Entities;

namespace FlickFrenzyBot_Web_App.Abstractions
{
    public interface IReviewRepository : IRepository<Review>
    {
        Review? GetByIds(int userId, int movieId);
    }
}
