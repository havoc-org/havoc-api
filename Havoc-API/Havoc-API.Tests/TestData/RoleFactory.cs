using Havoc_API.Models;

namespace Havoc_API.Tests.TestData;

public static class RoleFactory
{
    public static Role OwnerRole()
    {
        return new Role(RoleType.Owner);
    }

    public static Role ManagerRole()
    {
        return new Role(RoleType.Manager);
    }

    public static Role DevRole()
    {
        return new Role(RoleType.Developer);
    }
}