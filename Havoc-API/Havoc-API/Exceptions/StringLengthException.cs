namespace Havoc_API.Exceptions
{
    public class StringLengthException(Object o) : DomainException("Wrong argument length: " + o);
}
