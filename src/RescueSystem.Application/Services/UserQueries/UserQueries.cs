using Microsoft.EntityFrameworkCore;
using RescueSystem.Application.Contracts;
using RescueSystem.Application.DTO;

namespace RescueSystem.Application.Services.UserQueries;

public class UserQueries(IApplicationDbContext dbContext)
{
    public async Task<UserDto?> GetUserByIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var userDto = await dbContext.Users
            .Where(u => u.Id == userId)
            .Select(user => new UserDto(
                user.Id,
                user.FullName,
                user.DateOfBirth,
                user.RegistrationDate,
                user.MedicalNotes,
                user.EmergencyContact,
                user.BraceletId))
            .FirstOrDefaultAsync(cancellationToken);

        return userDto;
    }

    public async Task<(IReadOnlyList<UserDto> Items, int TotalCount)> GetPagedUsersAsync(int pageNumber, int pageSize, CancellationToken cancellationToken = default)
    {
        var totalCount = await dbContext.Users.CountAsync(cancellationToken);

        var userDtos = await dbContext.Users
            .OrderBy(u => u.FullName)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .Select(user => new UserDto(
                user.Id,
                user.FullName,
                user.DateOfBirth,
                user.RegistrationDate,
                user.MedicalNotes,
                user.EmergencyContact,
                user.BraceletId))
            .ToListAsync(cancellationToken);

        return (userDtos, totalCount);
    }
}
