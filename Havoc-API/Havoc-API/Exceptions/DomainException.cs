namespace Havoc_API.Exceptions;

public class DomainException : SupportedException
{
    public DomainException()
    {
    }

    public DomainException(string? message) : base(message)
    {
    }

    public DomainException(string? message, Exception? innerException) : base(message, innerException)
    {
    }
}