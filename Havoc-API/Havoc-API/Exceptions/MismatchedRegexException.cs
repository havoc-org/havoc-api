
namespace Havoc_API.Exceptions
{
    public class MismatchedRegexException(Object o) : DomainException("Mismatched regex: " + o);
}