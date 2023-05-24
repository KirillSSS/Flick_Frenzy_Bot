using FlickFrenzyBot_Web_App.Abstractions;
using FlickFrenzyBot_Web_App.Entities;
using Microsoft.EntityFrameworkCore;

namespace FlickFrenzyBot_Web_App.Database.Repositories
{
    public class MovieRepository : Repository<Movie>, IMovieRepository
    {
        public MovieRepository(BotDbContext dbContext) : base(dbContext)
        {
        }

        public Movie GetByTitle(string title)
        {
            return _dbSet.FirstOrDefault(e => e.Title == title);
        }
    }
}
