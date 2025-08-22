using RescueSystem.Application.Commands;
using RescueSystem.Application.Contracts;
using RescueSystem.Application.Exceptions;
using RescueSystem.Contracts.Contracts.Enums;
using RescueSystem.Domain.Entities.Bracelets;
using RescueSystem.Domain.Entities.Users;

namespace RescueSystem.Application.Services.UserService;

public class UserUseCases(IUserRepository userRepository, IBraceletRepository braceletRepository)
{
    public async Task<Guid> RegisterUserAsync(RegisterUserCommand command, CancellationToken cancellationToken = default)
    {
        var user = User.Create(
            command.FullName,
            command.DateOfBirth,
            command.MedicalNotes,
            command.EmergencyContact);

        await userRepository.AddAsync(user, cancellationToken);

        return user.Id;
    }

    public async Task AttachBraceletAsync(AttachBraceletCommand command, CancellationToken cancellationToken = default)
    {
        var user = await userRepository.GetByIdAsync(command.UserId, cancellationToken);
        if (user == null)
            throw new NotFoundException(nameof(User), command.UserId);

        var bracelet = await braceletRepository.GetByIdAsync(command.BraceletId, cancellationToken);
        if (bracelet == null)
            throw new NotFoundException(nameof(Bracelet), command.BraceletId);

        user.AttachBracelet(command.BraceletId);
        bracelet.ChangeStatus(BraceletStatus.Active);
        bracelet.AssignUser(user.Id);
    }

    public async Task DetachBraceletAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var user = await userRepository.GetByIdAsync(userId, cancellationToken);
        if (user == null)
            throw new NotFoundException(nameof(User), userId);

        var braceletId = user.BraceletId;
        if (braceletId == null)
            throw new BadRequestException($"User {userId} has no bracelet attached");

        var bracelet = await braceletRepository.GetByIdAsync(braceletId.Value, cancellationToken);
        if (bracelet == null)
            throw new NotFoundException(nameof(Bracelet), braceletId);

        user.DetachBracelet();
        bracelet.UnassignUser(user.Id);
    }
}