namespace Havoc_API.DTOs.ProjectStatus
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
