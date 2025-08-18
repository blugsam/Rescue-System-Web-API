using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using RescueSystem.Domain.Entities.Bracelets;
using RescueSystem.Domain.Interfaces;
using RescueSystem.Infrastructure.Data;

namespace RescueSystem.Infrastructure.Repositories;

public class BraceletRepository : IBraceletRepository
{
    private readonly ApplicationDbContext _dbContext;

    public BraceletRepository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Bracelet?> GetByIdWithUserAsync(Guid id)
    {
        return await _dbContext.Bracelets
                                    .Include(b => b.User)
                                    .FirstOrDefaultAsync(b => b.Id == id);
    }

    public async Task<Bracelet?> GetBraceletBySerialNumber(string serialNumber)
    {
        return await _dbContext.Bracelets
                                    .Include(b => b.User)
                                    .FirstOrDefaultAsync(b => b.SerialNumber == serialNumber);
    }

    public async Task<IEnumerable<Bracelet>> FindAsync(Expression<Func<Bracelet, bool>> predicate)
    {   
        return await _dbContext.Bracelets.Where(predicate).ToListAsync();
    }

    public async Task<IEnumerable<Bracelet>> GetAllAsync()
    {
        return await _dbContext.Bracelets.ToListAsync();
    }

    public async Task AddAsync(Bracelet entity)
    {
        await _dbContext.Bracelets.AddAsync(entity);
    }

    public void Remove(Bracelet entity)
    {
        _dbContext.Bracelets.Remove(entity);
    }

    public async Task<int> SaveChangesAsync()
    {
        return await _dbContext.SaveChangesAsync();
    }

    public async Task<Bracelet?> GetByIdAsync(Guid id)
    {
        return await _dbContext.Bracelets.FindAsync(id);
    }
}
