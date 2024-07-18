using Havoc_API.Models.DTOs.Participation;
using Havoc_API.Models.DTOs.ProjectStatus;
using System.ComponentModel.DataAnnotations;

namespace Havoc_API.Models.DTOs.Project
{
    public class ProjectPOST
    {

        [Required][MaxLength(25)] public string Name { get; set; } = null!;

        [MaxLength(200)] public string? Description { get; set; }

        public byte[]? Background { get; set; }

        [Required] public int CreatorId { get; set; }

        public DateTime? Start { get; set; }

        public DateTime? Deadline { get; set; }

        public virtual ProjectStatusPOST ProjectStatus { get; set; } = null!;

        public virtual ICollection<NewProjectParticipationPOST> Participations { get; set; } = new List<NewProjectParticipationPOST>();

        //public virtual ICollection<Task> Tasks { get; set; } = new List<Task>();

    }
}
