
using Havoc_API.Models.DTOs.Participation;
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
        public virtual ICollection<ParticipationGET> Participations { get; private set; } = new List<ParticipationGET>();
        public ProjectGET(int projectId, string name, string? description, byte[]? background, DateTime? start, DateTime? deadline, DateTime lastModified, UserGET creator, ProjectStatusGET projectStatus, ICollection<ParticipationGET> participations)
        {
            this.ProjectId = projectId;
            this.Name = name;
            this.Description = description;
            this.Background = background;
            this.Start = start;
            this.Deadline = deadline;
            this.LastModified = lastModified;
            this.Creator = creator;
            this.ProjectStatus = projectStatus;
            this.Participations = participations;
        }
    }
}
