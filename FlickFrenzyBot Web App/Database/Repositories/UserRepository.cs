using FlickFrenzyBot_Web_App.Abstractions;
using FlickFrenzyBot_Web_App.Entities;

namespace FlickFrenzyBot_Web_App.Database.Repositories
{
    public class UserRepository : Repository<User>, IUserRepository
    {
        public UserRepository(BotDbContext dbContext) : base(dbContext)
        {
        }

        public User? GetByNickname(string nickname)
        {
            return _dbSet.FirstOrDefault(e => e.Nickname == nickname);
        }
    }
}
