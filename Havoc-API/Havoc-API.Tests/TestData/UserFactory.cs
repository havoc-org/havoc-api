using Havoc_API.DTOs.User;
using Havoc_API.Models;

namespace Havoc_API.Tests.TestData;

public static class UserFactory
{
    public static User Create(string email = "test@test.test", string password = "testPass") =>
        new("TestName", "TestLastName", email, password);

    public static UserPOST CreatePost
    (
        string email = "test@test.test",
        string firstName = "TestName",
        string lastName = "TestLastName",
        string password = "testPass"
    ) =>
        new()
        {
            FirstName = firstName,
            LastName = lastName,
            Email = email,
            Password = password
        };

    public static UserGET CreateGet(User user)
    {
        return new UserGET(user.UserId, user.FirstName, user.LastName, user.Email);
    }
}