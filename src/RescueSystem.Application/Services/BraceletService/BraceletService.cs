using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using RescueSystem.Api.Exceptions;
using RescueSystem.Contracts.Contracts.Requests;
using RescueSystem.Contracts.Contracts.Responses;
using RescueSystem.Contracts.Contracts.Enums;
using RescueSystem.Application.Exceptions;
using RescueSystem.Domain.Entities;
using RescueSystem.Domain.Entities.Bracelets;
using RescueSystem.Infrastructure;

namespace RescueSystem.Application.Services.BraceletService;

public class BraceletService : IBraceletService
{
    private readonly ApplicationDbContext _db;
    private readonly IMapper _mapper;
    private readonly ILogger<BraceletService> _logger;

    public BraceletService(ApplicationDbContext db, IMapper mapper, ILogger<BraceletService> logger)
    {
        _db = db;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<BraceletDetailsDto> CreateBraceletAsync(CreateBraceletRequestDto request)
    {
        if (await _db.Bracelets.AnyAsync(b => b.SerialNumber == request.SerialNumber))
        {
            throw new BadRequestException($"Bracelet with serial '{request.SerialNumber}' already exist.");
        }

        var bracelet = _mapper.Map<Bracelet>(request);
        bracelet.Id = Guid.NewGuid();
        bracelet.Status = BraceletStatus.Inactive;

        _db.Bracelets.Add(bracelet);
        await _db.SaveChangesAsync();

        _logger.LogInformation("Created new bracelet. ID: {BraceletId}, Serial: {SerialNumber}", bracelet.Id, bracelet.SerialNumber);

        return await _db.Bracelets
            .AsNoTracking()
            .Where(b => b.Id == bracelet.Id)
            .ProjectTo<BraceletDetailsDto>(_mapper.ConfigurationProvider)
            .SingleAsync();
    }

    public async Task<BraceletDetailsDto?> GetBraceletByIdAsync(Guid braceletId)
    {
        return await _db.Bracelets
            .AsNoTracking()
            .Where(b => b.Id == braceletId)
            .ProjectTo<BraceletDetailsDto>(_mapper.ConfigurationProvider)
            .SingleOrDefaultAsync();
    }

    public async Task<PagedResult<BraceletDto>> GetAllBraceletsAsync(PaginationQueryParameters queryParams)
    {
        int pageNumber = queryParams.PageNumber;
        int pageSize = queryParams.PageSize;

        IQueryable<Bracelet> query = _db.Bracelets.AsNoTracking();

        if (!string.IsNullOrEmpty(queryParams.SearchTerm))
        {
            var term = queryParams.SearchTerm.Trim().ToLowerInvariant();
            query = query.Where(b => b.SerialNumber.ToLower().Contains(term));
        }

        var totalCount = await query.CountAsync();

        if (!string.IsNullOrEmpty(queryParams.SortBy))
        {
            switch (queryParams.SortBy.Trim().ToLowerInvariant())
            {
                case "serialnumber":
                    query = queryParams.SortDescending
                        ? query.OrderByDescending(b => b.SerialNumber)
                        : query.OrderBy(b => b.SerialNumber);
                    break;
                case "status":
                    query = queryParams.SortDescending
                        ? query.OrderByDescending(b => b.Status)
                        : query.OrderBy(b => b.Status);
                    break;
                default:
                    query = queryParams.SortDescending
                        ? query.OrderByDescending(b => b.SerialNumber)
                        : query.OrderBy(b => b.SerialNumber);
                    break;
            }
        }
        else
        {
            query = queryParams.SortDescending
                ? query.OrderByDescending(b => b.SerialNumber)
                : query.OrderBy(b => b.SerialNumber);
        }

        var items = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ProjectTo<BraceletDto>(_mapper.ConfigurationProvider)
            .ToListAsync();

        return new PagedResult<BraceletDto>
        {
            Items = items,
            PageNumber = pageNumber,
            PageSize = pageSize,
            TotalCount = totalCount
        };
    }

    public async Task UpdateBraceletStatusAsync(Guid braceletId, UpdateBraceletRequestDto request)
    {
        var bracelet = await _db.Bracelets.FirstOrDefaultAsync(b => b.Id == braceletId);
        if (bracelet == null)
            throw new NotFoundException($"Bracelet with ID '{braceletId}' not found.");

        if (!Enum.TryParse<BraceletStatus>(request.Status, true, out var newStatus))
            throw new BadRequestException($"Forbidden status value: '{request.Status}'.");

        var oldStatus = bracelet.Status;
        bracelet.Status = newStatus;

        await _db.SaveChangesAsync();
        _logger.LogInformation("Bracelet with ID {BraceletId} status changed from {OldStatus} to {NewStatus}", bracelet.Id, oldStatus, newStatus);
    }

    public async Task DeleteBraceletAsync(Guid braceletId)
    {
        var bracelet = await _db.Bracelets.FirstOrDefaultAsync(b => b.Id == braceletId);
        if (bracelet == null)
            throw new NotFoundException($"Bracelet with ID '{braceletId}' not found.");

        if (bracelet.UserId.HasValue)
            throw new BadRequestException("Can't delete bracelet, which attached to user.");

        _db.Bracelets.Remove(bracelet);
        await _db.SaveChangesAsync();
        _logger.LogWarning("Bracelet with ID {BraceletId} (Serial: {SerialNumber}) deleted.", bracelet.Id, bracelet.SerialNumber);
    }

    public async Task AssignUserToBraceletAsync(Guid braceletId, Guid userId)
    {
        var bracelet = await _db.Bracelets.FirstOrDefaultAsync(b => b.Id == braceletId);
        if (bracelet == null)
            throw new NotFoundException($"Bracelet with ID '{braceletId}' not found");

        if (!await _db.Users.AnyAsync(u => u.Id == userId))
            throw new NotFoundException($"User with ID '{userId}' not found.");

        if (bracelet.UserId.HasValue)
            throw new BadRequestException("This bracelet already attached to another user.");

        if (await _db.Bracelets.AnyAsync(b => b.UserId == userId))
            throw new BadRequestException("У этого пользователя уже есть другой браслет.");

        bracelet.UserId = userId;
        bracelet.Status = BraceletStatus.Active;
        await _db.SaveChangesAsync();
        _logger.LogInformation("User with ID {UserId} attached to bracelet with ID {BraceletId}", userId, braceletId);
    }

    public async Task UnassignUserFromBraceletAsync(Guid braceletId)
    {
        var bracelet = await _db.Bracelets.FirstOrDefaultAsync(b => b.Id == braceletId);
        if (bracelet == null)
            throw new NotFoundException($"Bracelt with ID '{braceletId}' not found.");

        if (!bracelet.UserId.HasValue)
            return;

        var userId = bracelet.UserId;
        bracelet.UserId = null;
        bracelet.Status = BraceletStatus.Inactive;
        await _db.SaveChangesAsync();
        _logger.LogInformation("User with ID {UserId} unattached from bracelet {BraceletId}", userId, braceletId);
    }
}