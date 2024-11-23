using Havoc_API.Models;

namespace Havoc_API.Tests.TestData;

public static class ProjectFactory
{
    public static Project Create(User creator)
    {
        return new Project(
            "TestName",
            "Test description",
            new byte[1234],
            DateTime.Now,
            DateTime.Now.AddDays(34),
            creator,
            new ProjectStatus("TestProjectStatus")
        );
    }
}