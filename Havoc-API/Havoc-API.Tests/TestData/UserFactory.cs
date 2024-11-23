using Havoc_API.Models;

namespace Havoc_API.Tests.TestData;

public static class UserFactory
{
    public static User Create(string email = "test@test.test")
    {
        return new User("TestName", "TestLastName", email, "testPass");
    }
}