using Microsoft.Extensions.Logging;
using RescueSystem.Api.Exceptions;
using RescueSystem.Contracts.Contracts.Requests;
using RescueSystem.Contracts.Contracts.Responses;
using RescueSystem.Application.Exceptions;
using RescueSystem.Domain.Interfaces;
using RescueSystem.Application.Mapping;
using RescueSystem.Domain.Common;

namespace RescueSystem.Application.Services.UserService;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;
    private readonly ILogger<UserService> _logger;

    public UserService(IUserRepository userRepository, ILogger<UserService> logger)
    {
        _userRepository = userRepository;
        _logger = logger;
    }

    public async Task<UserDetailsDto> CreateUserAsync(CreateUserRequestDto request)
    {
        var user = request.ToEntity();
        user.Id = Guid.NewGuid();
        await _userRepository.AddAsync(user);
        await _userRepository.SaveChangesAsync();
        _logger.LogInformation("New user created. ID: {UserId}", user.Id);

        return user.ToDetailsDto();
    }

    public async Task<UserDetailsDto?> GetUserByIdAsync(Guid userId)
    {
        var user = await _userRepository.GetByIdAsync(userId);
        return user?.ToDetailsDto();
    }

    public async Task<PagedResult<UserSummaryDto>> GetAllUsersAsync(PaginationQueryParameters queryParams)
    {
        var users = await _userRepository.GetAllAsync();
        var totalCount = users.Count();

        if (!string.IsNullOrEmpty(queryParams.SearchTerm))
        {
            var lowerTerm = queryParams.SearchTerm.ToLowerInvariant();
            users = users.Where(u => u.FullName.ToLower().Contains(lowerTerm));
        }

        if (!string.IsNullOrEmpty(queryParams.SortBy))
        {
            var sortBy = queryParams.SortBy.Trim().ToLowerInvariant();
            switch (sortBy)
            {
                case "fullname":
                    users = queryParams.SortDescending
                        ? users.OrderByDescending(u => u.FullName)
                        : users.OrderBy(u => u.FullName);
                    break;
                case "dateofbirth":
                    users = queryParams.SortDescending
                        ? users.OrderByDescending(u => u.DateOfBirth)
                        : users.OrderBy(u => u.DateOfBirth);
                    break;
                default:
                    users = queryParams.SortDescending
                        ? users.OrderByDescending(u => u.FullName)
                        : users.OrderBy(u => u.FullName);
                    break;
            }
        }
        else
        {
            users = queryParams.SortDescending
                ? users.OrderByDescending(u => u.FullName)
                : users.OrderBy(u => u.FullName);
        }

        var items = users
            .Skip((queryParams.PageNumber - 1) * queryParams.PageSize)
            .Take(queryParams.PageSize)
            .ToList();

        var dtos = items.Select(i => i.ToSummaryDto()).ToList();

        return new PagedResult<UserSummaryDto>
        {
            Items = dtos,
            PageNumber = queryParams.PageNumber,
            PageSize = queryParams.PageSize,
            TotalCount = totalCount
        };
    }


    public async Task<UserDetailsDto> UpdateUserAsync(Guid userId, UpdateUserRequestDto request)
    {
        var user = await _userRepository.GetByIdAsync(userId);
        if (user == null)
            throw new NotFoundException($"User '{userId}' has not found.");

        user.UpdateEntity(request);
        await _userRepository.SaveChangesAsync();
        _logger.LogInformation("User '{UserId}' data updated.", userId);

        return user.ToDetailsDto();
    }

    public async Task DeleteUserAsync(Guid userId)
    {
        var user = await _userRepository.GetByIdAsync(userId);
        if (user == null)
            throw new NotFoundException($"User '{userId}' has not found.");

        if (user.Bracelet != null)
        {
            throw new BadRequestException("You can't delete user with assigned bracelet.");
        }

        _userRepository.Remove(user);
        await _userRepository.SaveChangesAsync();
        _logger.LogWarning("User '{UserId}' has been deleted.", user.Id);
    }
}