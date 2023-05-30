using FlickFrenzyBot_Web_App.Abstractions;
using FlickFrenzyBot_Web_App.Entities;

namespace FlickFrenzyBot_Web_App.Database.Repositories
{
    public class StateRepository : Repository<CurrentState>, IStateRepository
    {
        public StateRepository(BotDbContext dbContext) : base(dbContext)
        {
        }

        public CurrentState? GetByUserId(int id)
        {
            return _dbSet.FirstOrDefault(e => e.UserId == id);
        }
    }
}
