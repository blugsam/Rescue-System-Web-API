namespace RescueSystem.Application.Exceptions;

public class BadRequestException : RescueSystemException
{
    public IDictionary<string, string[]>? Errors { get; }

    public BadRequestException(string message, IDictionary<string, string[]>? errors = null) : base(message)
    {
        Errors = errors;
    }
}