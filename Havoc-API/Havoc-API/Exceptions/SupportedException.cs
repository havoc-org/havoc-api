public abstract class SupportedException : Exception
{
    public SupportedException()
    {
    }

    public SupportedException(string? message) : base(message)
    {
    }

    public SupportedException(string? message, Exception? innerException) : base(message, innerException)
    {
    }
}