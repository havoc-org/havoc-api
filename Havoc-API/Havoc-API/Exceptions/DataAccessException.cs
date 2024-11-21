
namespace Havoc_API.Exceptions;

public class DataAccessException : SupportedException
{
    public DataAccessException()
    {
    }

    public DataAccessException(string? message) : base(message)
    {
    }

    public DataAccessException(string? message, Exception? innerException) : base(message, innerException)
    {
    }
}