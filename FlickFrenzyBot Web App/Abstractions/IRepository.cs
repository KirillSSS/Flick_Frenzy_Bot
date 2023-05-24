namespace FlickFrenzyBot_Web_App.Abstractions
{
    public interface IRepository<T> where T : class
    {
        T GetById(int id);
        void Create(T entity);
        void Update(T entity);
        void Delete(T entity);
        IEnumerable<T> GetAll();
    }
}
