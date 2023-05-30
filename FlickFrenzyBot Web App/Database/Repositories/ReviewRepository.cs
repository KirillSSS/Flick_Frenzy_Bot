using FlickFrenzyBot_Web_App.Abstractions;
using FlickFrenzyBot_Web_App.Entities;

namespace FlickFrenzyBot_Web_App.Database.Repositories
{
    public class ReviewRepository : Repository<Review>, IReviewRepository
    {
        public ReviewRepository(BotDbContext dbContext) : base(dbContext)
        {
        }

        public Review? GetByIds(int userId, int movieId)
        {
            return _dbSet.FirstOrDefault(e => (e.UserId == userId && e.MovieId == movieId));
        }
    }
}
