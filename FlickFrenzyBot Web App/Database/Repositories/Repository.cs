using FlickFrenzyBot_Web_App.Abstractions;
using FlickFrenzyBot_Web_App.Database;
using FlickFrenzyBot_Web_App.Entities;
using Microsoft.EntityFrameworkCore;

public class Repository<T> : IRepository<T> where T : class
{
    private readonly BotDbContext _dbContext;
    public readonly DbSet<T> _dbSet;

    public Repository(BotDbContext dbContext)
    {
        _dbContext = dbContext;
        _dbSet = _dbContext.Set<T>();
    }

    public T? GetById(int id)
    {
        return _dbSet.Find(id);
    }

    public void DeleteById(int id)
    {
        var entity = _dbSet.Find(id);
        if (entity != null)
        {
            _dbSet.Remove(entity);  
        }
        _dbContext.SaveChanges();
    }

    public void Update(T entity)
    {
        _dbContext.Entry(entity).State = EntityState.Modified;
        _dbContext.SaveChanges();
    }

    public void Create(T entity)
    {
        _dbSet.Add(entity);
        _dbContext.SaveChanges();
    }

    public void Delete(T entity)
    {
        _dbSet.Remove(entity);
        _dbContext.SaveChanges();
    }

    public IEnumerable<T> GetAll()
    {
        return _dbSet.ToList();
    }
}
