using Microsoft.EntityFrameworkCore;
using RescueSystem.Domain.Interfaces;
using System.Linq.Expressions;

namespace RescueSystem.Infrastructure;

public class Repository<T> : IRepository<T> where T : class
{
    private readonly ApplicationDbContext _dbContext;

    public Repository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<T?> GetByIdAsync(Guid id)
    {
        return await _dbContext.Set<T>().FindAsync(id);
    }

    public async Task<IEnumerable<T>> GetAllAsync()
    {
        return await _dbContext.Set<T>().ToListAsync();
    }

    public async Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate)
    {
        return await _dbContext.Set<T>().Where(predicate).ToListAsync();
    }

    public async Task AddAsync(T entity)
    {
        await _dbContext.Set<T>().AddAsync(entity);
    }

    public void Remove(T entity)
    {
        _dbContext.Set<T>().Remove(entity);
    }

    public async Task<int> SaveChangesAsync()
    {
        return await _dbContext.SaveChangesAsync();
    }
}
