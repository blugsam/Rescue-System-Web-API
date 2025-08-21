using RescueSystem.Application.Contracts;
using RescueSystem.Application.Exceptions;
using RescueSystem.Contracts.Contracts.Enums;
using RescueSystem.Domain.Entities.Bracelets;

namespace RescueSystem.Application.Services.BraceletService;

public class BraceletUseCases(IBraceletRepository braceletRepository)
{
    public async Task<Guid> RegisterBraceletAsync(string SerialNumber, CancellationToken cancellationToken = default)
    {
        var bracelet = Bracelet.Create(SerialNumber);

        await braceletRepository.AddAsync(bracelet, cancellationToken);

        return bracelet.Id;
    }

    public async Task DeleteBraceletAsync(Guid braceletId, CancellationToken cancellationToken = default)
    {
        var bracelet = await braceletRepository.GetByIdAsync(braceletId);
        if (bracelet == null)
            throw new NotFoundException(nameof(bracelet), braceletId);

        if (bracelet.UserId.HasValue)
            throw new BadRequestException($"Bracelet with ID {bracelet.Id} already assigned to user {bracelet.UserId}");

        await braceletRepository.DeleteAsync(bracelet, cancellationToken);
    }

    public async Task ChangeStatusAsync(Guid braceletId, BraceletStatus newStatus, CancellationToken cancellationToken = default)
    {
        var bracelet = await braceletRepository.GetByIdAsync(braceletId);
        if (bracelet == null)
            throw new NotFoundException(nameof(Bracelet), braceletId);

        bracelet.ChangeStatus(newStatus);
    }
}