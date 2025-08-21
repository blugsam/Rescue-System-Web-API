using Microsoft.EntityFrameworkCore;
using RescueSystem.Application.Contracts;
using RescueSystem.Application.DTO;

namespace RescueSystem.Application.Services.BraceletService;

public class BraceletQueries(IApplicationDbContext dbContext)
{
    public async Task<BraceletDto?> GetBraceletByIdAsync(Guid braceletId, CancellationToken cancellationToken = default)
    {
        var braceletDto = await dbContext.Bracelets
            .Where(u => u.Id == braceletId)
            .Select(bracelet => new BraceletDto(
                bracelet.Id,
                bracelet.SerialNumber,
                bracelet.RegistrationInSystemDate,
                bracelet.LastRepairDate,
                bracelet.Status,
                bracelet.UserId))
            .FirstOrDefaultAsync(cancellationToken);

        return braceletDto;
    }

    public async Task<(IReadOnlyList<BraceletDto> Items, int TotalCount)> GetPagedBraceletsAsync(int pageNumber, int pageSize, CancellationToken cancellationToken = default)
    {
        var totalCount = await dbContext.Bracelets.CountAsync(cancellationToken);

        var braceletDtos = await dbContext.Bracelets
            .OrderBy(u  => u.SerialNumber)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .Select(bracelet => new BraceletDto(
                bracelet.Id,
                bracelet.SerialNumber,
                bracelet.RegistrationInSystemDate,
                bracelet.LastRepairDate,
                bracelet.Status,
                bracelet.UserId))
            .ToListAsync(cancellationToken);

        return (braceletDtos, totalCount);
    }
}
