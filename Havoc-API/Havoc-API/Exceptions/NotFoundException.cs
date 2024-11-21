namespace Havoc_API.Exceptions
{
    public class NotFoundException : SupportedException
    {
        public NotFoundException()
        {
        }
        public NotFoundException(string? message) : base(message)
        {
        }

        public NotFoundException(string? message, Exception? innerException) : base(message, innerException)
        {
        }
    }
}
