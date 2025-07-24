using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using RescueSystem.Api.Exceptions;
using RescueSystem.Contracts.Contracts.Requests;
using RescueSystem.Contracts.Contracts.Responses;
using RescueSystem.Application.Exceptions;
using RescueSystem.Domain.Entities;
using RescueSystem.Infrastructure;

namespace RescueSystem.Application.Services.UserService;

public class UserService : IUserService
{
    private readonly ApplicationDbContext _db;
    private readonly IMapper _mapper;
    private readonly ILogger<UserService> _logger;

    public UserService(ApplicationDbContext db, IMapper mapper, ILogger<UserService> logger)
    {
        _db = db;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<UserDetailsDto> CreateUserAsync(CreateUserRequestDto request)
    {
        var user = _mapper.Map<User>(request);
        user.Id = Guid.NewGuid();
        _db.Users.Add(user);
        await _db.SaveChangesAsync();
        _logger.LogInformation("New user created. ID: {UserId}", user.Id);

        return await _db.Users.AsNoTracking()
            .Where(u => u.Id == user.Id)
            .ProjectTo<UserDetailsDto>(_mapper.ConfigurationProvider)
            .SingleAsync();
    }

    public async Task<UserDetailsDto?> GetUserByIdAsync(Guid userId)
    {
        return await _db.Users
            .AsNoTracking()
            .Where(u => u.Id == userId)
            .ProjectTo<UserDetailsDto>(_mapper.ConfigurationProvider)
            .SingleOrDefaultAsync();
    }

    public async Task<PagedResult<UserSummaryDto>> GetAllUsersAsync(PaginationQueryParameters queryParams)
    {
        int pageNumber = queryParams.PageNumber;
        int pageSize = queryParams.PageSize;

        IQueryable<User> query = _db.Users.AsNoTracking();

        if (!string.IsNullOrEmpty(queryParams.SearchTerm))
        {
            var pattern = $"%{queryParams.SearchTerm}%";
            query = query.Where(u => EF.Functions.ILike(u.FullName, pattern));

            //var lowerTerm = queryParams.SearchTerm.ToLowerInvariant();
            //query = query.Where(u => u.FullName.ToLower().Contains(lowerTerm));
        }

        var totalCount = await query.CountAsync();

        if (!string.IsNullOrEmpty(queryParams.SortBy))
        {
            var sortBy = queryParams.SortBy.Trim().ToLowerInvariant();
            switch (sortBy)
            {
                case "fullname":
                    query = queryParams.SortDescending
                        ? query.OrderByDescending(u => u.FullName)
                        : query.OrderBy(u => u.FullName);
                    break;
                case "dateofbirth":
                    query = queryParams.SortDescending
                        ? query.OrderByDescending(u => u.DateOfBirth)
                        : query.OrderBy(u => u.DateOfBirth);
                    break;
                default:
                    query = queryParams.SortDescending
                        ? query.OrderByDescending(u => u.FullName)
                        : query.OrderBy(u => u.FullName);
                    break;
            }
        }
        else
        {
            query = queryParams.SortDescending
                ? query.OrderByDescending(u => u.FullName)
                : query.OrderBy(u => u.FullName);
        }

        var items = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ProjectTo<UserSummaryDto>(_mapper.ConfigurationProvider)
            .ToListAsync();

        return new PagedResult<UserSummaryDto>
        {
            Items = items,
            PageNumber = pageNumber,
            PageSize = pageSize,
            TotalCount = totalCount
        };
    }


    public async Task<UserDetailsDto> UpdateUserAsync(Guid userId, UpdateUserRequestDto request)
    {
        var user = await _db.Users.FirstOrDefaultAsync(u => u.Id == userId);
        if (user == null)
            throw new NotFoundException($"User '{userId}' has not found.");

        _mapper.Map(request, user);
        await _db.SaveChangesAsync();
        _logger.LogInformation("User '{UserId}' data updated.", userId);

        return await _db.Users.AsNoTracking().Where(u => u.Id == userId)
            .ProjectTo<UserDetailsDto>(_mapper.ConfigurationProvider).SingleAsync();
    }

    public async Task DeleteUserAsync(Guid userId)
    {
        var user = await _db.Users.Include(u => u.Bracelet).FirstOrDefaultAsync(u => u.Id == userId);
        if (user == null)
            throw new NotFoundException($"User '{userId}' has not found.");

        if (user.Bracelet != null)
        {
            throw new BadRequestException("You can't delete user with assigned bracelet.");
        }

        _db.Users.Remove(user);
        await _db.SaveChangesAsync();
        _logger.LogWarning("User '{UserId}' has been deleted.", user.Id);
    }
}