using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using RescueSystem.Domain.Entities.Health;
using RescueSystem.Domain.Interfaces;
using RescueSystem.Infrastructure.Data;

namespace RescueSystem.Infrastructure.Repositories;

public class HealthProfileThresholdsRepository : IHealthProfileThresholdsRepository
{
    private readonly ApplicationDbContext _dbContext;

    public HealthProfileThresholdsRepository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<HealthProfileThresholds?> FindAsync(Expression<Func<HealthProfileThresholds, bool>> predicate)
    {
        return await _dbContext.HealthProfileThresholds.FirstOrDefaultAsync(predicate);
    }
}