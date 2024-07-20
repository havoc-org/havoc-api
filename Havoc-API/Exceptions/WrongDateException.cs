using WarehouseApp2.Exceptions;

namespace Havoc_API.Exceptions
{
    public class WrongDateException(Object o) : DomainException("Wrong Date: " + o);
}
