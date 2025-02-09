using Havoc_API.DTOs.Participation;
using Havoc_API.DTOs.ProjectStatus;
using Havoc_API.DTOs.User;

namespace Havoc_API.DTOs.Project
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
        public string InviteCode { get; set; } = null!;
        
        public ProjectGET(int projectId, string name, string? description, byte[]? background, DateTime? start, DateTime? deadline, DateTime lastModified, UserGET creator, ProjectStatusGET projectStatus, ICollection<ParticipationGET> participations)
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
            Participations = participations;
        }

        public ProjectGET(int projectId, string name, string? description, byte[]? background, DateTime? start, DateTime? deadline, DateTime lastModified)
        {
            ProjectId = projectId;
            Name = name;
            Description = description;
            Background = background;
            Start = start;
            Deadline = deadline;
            LastModified = lastModified;
        }
    }
}
