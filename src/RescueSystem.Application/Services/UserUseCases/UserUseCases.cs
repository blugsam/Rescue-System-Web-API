using RescueSystem.Application.Commands;
using RescueSystem.Application.Contracts;
using RescueSystem.Application.Exceptions;
using RescueSystem.Domain.Entities.Bracelets;
using RescueSystem.Domain.Entities.Users;

namespace RescueSystem.Application.Services.UserUseCases;

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
        {
            throw new NotFoundException(nameof(User), command.UserId);
        }

        var bracelet = await braceletRepository.GetByIdAsync(command.BraceletId, cancellationToken);
        if (bracelet == null)
        {
            throw new NotFoundException(nameof(Bracelet), command.BraceletId);
        }

        if (bracelet.UserId is not null)
        {
            throw new InvalidOperationException($"Bracelet {bracelet.Id} is already attached to user {bracelet.UserId}.");
        }

        user.AttachBracelet(command.BraceletId);

        await userRepository.UpdateAsync(user, cancellationToken);
    }
}