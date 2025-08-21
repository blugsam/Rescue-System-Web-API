using RescueSystem.Application.Contracts;
using RescueSystem.Application.DTO;

namespace RescueSystem.Application.Services.QueryService;

public class UserQueries(IUserRepository userRepository)
{
    public async Task<UserDto?> GetUserByIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var user = await userRepository.GetByIdAsync(userId, cancellationToken);

        if (user is null)
        {
            return null;
        }

        return new UserDto(
            user.Id,
            user.FullName,
            user.DateOfBirth,
            user.RegistrationDate,
            user.MedicalNotes,
            user.EmergencyContact,
            user.BraceletId
        );
    }

    public async Task<(IReadOnlyList<UserDto> Items, int TotalCount)> GetPagedUsersAsync(int pageNumber, int pageSize, CancellationToken cancellationToken = default)
    {
        var (users, totalCount) = await userRepository.GetPagedAsync(pageNumber, pageSize, cancellationToken);

        var userDtos = users
            .Select(user => new UserDto(
                user.Id,
                user.FullName,
                user.DateOfBirth,
                user.RegistrationDate,
                user.MedicalNotes,
                user.EmergencyContact,
                user.BraceletId))
            .ToList();

        return (userDtos, totalCount);
    }
}
