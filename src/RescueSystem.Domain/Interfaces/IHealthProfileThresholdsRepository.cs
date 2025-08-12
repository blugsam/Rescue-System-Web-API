using RescueSystem.Domain.Entities.Health;
using System.Linq.Expressions;

namespace RescueSystem.Domain.Interfaces;

public interface IHealthProfileThresholdsRepository
{
    Task<HealthProfileThresholds?> FindAsync(Expression<Func<HealthProfileThresholds, bool>> predicate);
}