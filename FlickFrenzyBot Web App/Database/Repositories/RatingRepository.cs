using FlickFrenzyBot_Web_App.Abstractions;
using FlickFrenzyBot_Web_App.Entities;
using Microsoft.EntityFrameworkCore;

namespace FlickFrenzyBot_Web_App.Database.Repositories
{
    public class RatingRepository : Repository<Rating>, IRatingRepository
    {
        public RatingRepository(BotDbContext dbContext) : base(dbContext)
        {
        }

        public List<Rating> GetAllByMovieId(int movieId)
        {
            return _dbSet.Where(r => r.MovieId == movieId).ToList();
        }
    }
}
