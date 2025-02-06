using Havoc_API.DTOs.Participation;
using Havoc_API.Models;

namespace Havoc_API.Tests.TestData;

public static class ParticipationFactory
{
    public static Participation Create(Project project, Role role, User user)
    {
        return new Participation(project, role, user);
    }

    public static ParticipationPOST CreatePost(int projectId = 0, string email = "user@user.gmail", RoleType roleType = RoleType.Developer)
    {
        return new ParticipationPOST(projectId, email, roleType);
    }
}