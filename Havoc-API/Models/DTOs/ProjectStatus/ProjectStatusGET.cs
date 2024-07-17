namespace Havoc_API.Models.DTOs.ProjectStatus
{
    public class ProjectStatusGET
    {
        public int ProjectStatusId { get; private set; }

        public string Name { get; private set; } = null!;

        public ProjectStatusGET(int projectStatusId, string name)
        {
            ProjectStatusId = projectStatusId;
            Name = name;
        }
    }
}
