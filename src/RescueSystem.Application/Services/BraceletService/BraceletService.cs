using Microsoft.Extensions.Logging;
using RescueSystem.Api.Exceptions;
using RescueSystem.Contracts.Contracts.Requests;
using RescueSystem.Contracts.Contracts.Responses;
using RescueSystem.Contracts.Contracts.Enums;
using RescueSystem.Application.Exceptions;
using RescueSystem.Domain.Interfaces;
using RescueSystem.Application.Mapping.Bracelets;
using RescueSystem.Application.Mapping;
using RescueSystem.Domain.Common;

namespace RescueSystem.Application.Services.BraceletService;

public class BraceletService : IBraceletService
{
    private readonly IBraceletRepository _braceletRepository;
    private readonly IUserRepository _userRepository;
    private readonly ILogger<BraceletService> _logger;

    public BraceletService(IBraceletRepository braceletRepository, IUserRepository userRepository, ILogger<BraceletService> logger)
    {
        _braceletRepository = braceletRepository;
        _userRepository = userRepository;
        _logger = logger;
    }

    public async Task<BraceletDetailsDto> CreateBraceletAsync(CreateBraceletRequestDto request)
    {
        if ((await _braceletRepository.FindAsync(b => b.SerialNumber == request.SerialNumber)).Any())
        {
            throw new BadRequestException($"Bracelet with serial '{request.SerialNumber}' already exist.");
        }

        var bracelet = request.ToEntity();
        bracelet.Id = Guid.NewGuid();
        bracelet.Status = BraceletStatus.Inactive;

        await _braceletRepository.AddAsync(bracelet);
        await _braceletRepository.SaveChangesAsync();

        _logger.LogInformation("Created new bracelet. ID: {BraceletId}, Serial: {SerialNumber}", bracelet.Id, bracelet.SerialNumber);

        return bracelet.ToDetailsDto();
    }

    public async Task<BraceletDetailsDto?> GetBraceletByIdAsync(Guid braceletId)
    {
        var bracelet = await _braceletRepository.GetByIdWithUserAsync(braceletId);
        return bracelet?.ToDetailsDto();
    }

    public async Task<PagedResult<BraceletDto>> GetAllBraceletsAsync(PaginationQueryParameters queryParams)
    {
        var bracelets = await _braceletRepository.GetAllAsync();
        var totalCount = bracelets.Count();

        if (!string.IsNullOrEmpty(queryParams.SearchTerm))
        {
            var term = queryParams.SearchTerm.Trim().ToLowerInvariant();
            bracelets = bracelets.Where(b => b.SerialNumber.ToLower().Contains(term));
        }

        if (!string.IsNullOrEmpty(queryParams.SortBy))
        {
            switch (queryParams.SortBy.Trim().ToLowerInvariant())
            {
                case "serialnumber":
                    bracelets = queryParams.SortDescending
                        ? bracelets.OrderByDescending(b => b.SerialNumber)
                        : bracelets.OrderBy(b => b.SerialNumber);
                    break;
                case "status":
                    bracelets = queryParams.SortDescending
                        ? bracelets.OrderByDescending(b => b.Status)
                        : bracelets.OrderBy(b => b.Status);
                    break;
                default:
                    bracelets = queryParams.SortDescending
                        ? bracelets.OrderByDescending(b => b.SerialNumber)
                        : bracelets.OrderBy(b => b.SerialNumber);
                    break;
            }
        }
        else
        {
            bracelets = queryParams.SortDescending
                ? bracelets.OrderByDescending(b => b.SerialNumber)
                : bracelets.OrderBy(b => b.SerialNumber);
        }

        var items = bracelets
            .Skip((queryParams.PageNumber - 1) * queryParams.PageSize)
            .Take(queryParams.PageSize)
            .ToList();

        var dtos = items.Select(i => i.ToDto()).ToList();

        return new PagedResult<BraceletDto>
        {
            Items = dtos,
            PageNumber = queryParams.PageNumber,
            PageSize = queryParams.PageSize,
            TotalCount = totalCount
        };
    }

    public async Task UpdateBraceletStatusAsync(Guid braceletId, UpdateBraceletRequestDto request)
    {
        var bracelet = await _braceletRepository.GetByIdAsync(braceletId);
        if (bracelet == null)
            throw new NotFoundException($"Bracelet with ID '{braceletId}' not found.");

        if (!Enum.TryParse<BraceletStatus>(request.Status, true, out var newStatus))
            throw new BadRequestException($"Forbidden status value: '{request.Status}'.");

        var oldStatus = bracelet.Status;
        bracelet.Status = newStatus;

        await _braceletRepository.SaveChangesAsync();
        _logger.LogInformation("Bracelet with ID {BraceletId} status changed from {OldStatus} to {NewStatus}", bracelet.Id, oldStatus, newStatus);
    }

    public async Task DeleteBraceletAsync(Guid braceletId)
    {
        var bracelet = await _braceletRepository.GetByIdAsync(braceletId);
        if (bracelet == null)
            throw new NotFoundException($"Bracelet with ID '{braceletId}' not found.");

        if (bracelet.UserId.HasValue)
            throw new BadRequestException("Can't delete bracelet, which attached to user.");

        _braceletRepository.Remove(bracelet);
        await _braceletRepository.SaveChangesAsync();
        _logger.LogWarning("Bracelet with ID {BraceletId} (Serial: {SerialNumber}) deleted.", bracelet.Id, bracelet.SerialNumber);
    }

    public async Task AssignUserToBraceletAsync(Guid braceletId, Guid userId)
    {
        var bracelet = await _braceletRepository.GetByIdAsync(braceletId);
        if (bracelet == null)
            throw new NotFoundException($"Bracelet with ID '{braceletId}' not found");

        if (await _userRepository.GetByIdAsync(userId) == null)
            throw new NotFoundException($"User with ID '{userId}' not found.");

        var userToAssign = await _userRepository.GetByIdAsync(userId);
        if (userToAssign == null)
            throw new NotFoundException($"User with ID '{userId}' not found.");

        if (bracelet.UserId.HasValue)
            throw new BadRequestException("This bracelet already attached to another user.");

        if ((await _braceletRepository.FindAsync(b => b.UserId == userId)).Any())
            throw new BadRequestException($"User with ID '{userId}' already attached to another bracelet.");

        bracelet.UserId = userId;
        bracelet.User = userToAssign;
        bracelet.Status = BraceletStatus.Active;
        await _braceletRepository.SaveChangesAsync();
        _logger.LogInformation("User with ID {UserId} attached to bracelet with ID {BraceletId}", userId, braceletId);
    }

    public async Task UnassignUserFromBraceletAsync(Guid braceletId)
    {
        var bracelet = await _braceletRepository.GetByIdAsync(braceletId);
        if (bracelet == null)
            throw new NotFoundException($"Bracelt with ID '{braceletId}' not found.");

        if (!bracelet.UserId.HasValue)
            return;

        var userId = bracelet.UserId;
        bracelet.UserId = null;
        bracelet.Status = BraceletStatus.Inactive;
        await _braceletRepository.SaveChangesAsync();
        _logger.LogInformation("User with ID {UserId} unattached from bracelet {BraceletId}", userId, braceletId);
    }
}
