namespace RescueSystem.Application.Exceptions;

public class NotFoundException : RescueSystemException
{
    public NotFoundException(string message) : base(message) { }
}
