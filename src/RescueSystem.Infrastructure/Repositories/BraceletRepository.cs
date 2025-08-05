using Microsoft.EntityFrameworkCore;
using RescueSystem.Domain.Entities.Bracelets;
using RescueSystem.Domain.Interfaces;

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
}