using RescueSystem.Contracts.Contracts.Requests;
using RescueSystem.Contracts.Contracts.Responses;
using RescueSystem.Domain.Entities;

namespace RescueSystem.Application.Services.UserService;

public interface IUserService
{
    Task<UserDetailsDto> CreateUserAsync(CreateUserRequestDto request);

    Task<UserDetailsDto> UpdateUserAsync(Guid userId, UpdateUserRequestDto request);

    Task DeleteUserAsync(Guid userId);

    Task<UserDetailsDto?> GetUserByIdAsync(Guid userId);

    Task<PagedResult<UserSummaryDto>> GetAllUsersAsync(PaginationQueryParameters queryParams);
}