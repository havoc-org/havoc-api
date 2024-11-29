using Havoc_API.Models;

namespace Havoc_API.Tests.TestData;

public static class ParticipationFactory
{
    public static Participation Create(Project project, Role role, User user)
    {
        return new Participation(project, role, user);
    }
}