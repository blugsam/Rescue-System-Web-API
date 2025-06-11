using RescueSystem.Application.Contracts.Requests;
using RescueSystem.Application.Contracts.Responses;
using RescueSystem.Domain.Entities;
using RescueSystem.Application.Contracts;

namespace RescueSystem.Application.Services.UserService;

public interface IUserService
{
    Task<UserDetailsDto> CreateUserAsync(CreateUserRequest request);

    Task<UserDetailsDto> UpdateUserAsync(Guid userId, UpdateUserRequest request);

    Task DeleteUserAsync(Guid userId);

    Task<UserDetailsDto?> GetUserByIdAsync(Guid userId);

    Task<PagedResult<UserSummaryDto>> GetAllUsersAsync(PaginationQueryParameters queryParams);
}