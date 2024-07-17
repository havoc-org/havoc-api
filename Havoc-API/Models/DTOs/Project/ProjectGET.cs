
using Havoc_API.Models.DTOs.ProjectStatus;
using Havoc_API.Models.DTOs.User;
namespace Havoc_API.Models.DTOs.Project
{
    public class ProjectGET
    {
        public int ProjectId { get; private set; }

        public string Name { get; private set; } = null!;

        public string? Description { get; private set; }

        public byte[]? Background { get; private set; }

        public DateTime? Start { get; private set; }

        public DateTime? Deadline { get; private set; }

        public DateTime LastModified { get; private set; }

        public virtual UserGET Creator { get; private set; } = null!;

        public virtual ProjectStatusGET ProjectStatus { get; private set; } = null!;
        public ProjectGET(int projectId, string name, string? description, byte[]? background, DateTime? start, DateTime? deadline, DateTime lastModified, UserGET creator, ProjectStatusGET projectStatus)
        {
            ProjectId = projectId;
            Name = name;
            Description = description;
            Background = background;
            Start = start;
            Deadline = deadline;
            LastModified = lastModified;
            Creator = creator;
            ProjectStatus = projectStatus;
        }
    }
}
