namespace RescueSystem.Application.Exceptions;

public abstract class RescueSystemException : Exception
{
    public RescueSystemException(string message) : base(message) { }
}