namespace RescueSystem.Application.Exceptions;

public class BadRequestException : RescueSystemException
{
    public BadRequestException(string message) : base(message) { }
}