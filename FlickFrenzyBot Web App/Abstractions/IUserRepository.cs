using FlickFrenzyBot_Web_App.Entities;

namespace FlickFrenzyBot_Web_App.Abstractions
{
    public interface IUserRepository : IRepository<User>
    {
        User? GetByNickname(string nickname);
        int GetIdByNickname(string nickname);
    }
}
