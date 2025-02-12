using Havoc_API.DTOs.Participation;
using Havoc_API.DTOs.Project;
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

    public static ProjectGET CreateGet(Project project)
    {
        return new ProjectGET
        (
            project.ProjectId,
            project.Name,
            project.Description,
            project.Background,
            project.Start,
            project.Deadline,
            project.LastModified,
            UserFactory.CreateGet(project.Creator),
            new DTOs.ProjectStatus.ProjectStatusGET(project.ProjectStatus.ProjectStatusId, project.ProjectStatus.Name),
            project.Participations.Select(p => new ParticipationGET(project.ProjectId,
                new DTOs.User.UserParticipationGET(p.User.UserId, p.User.FirstName, p.User.LastName, p.User.Email, new DTOs.Role.RoleGET(p.Role.RoleId, p.Role.Name)))).ToList()
        );
    }
}