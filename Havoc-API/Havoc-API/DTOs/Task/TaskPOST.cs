using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Havoc_API.DTOs.Assignment;
using Havoc_API.DTOs.Attachment;
using Havoc_API.DTOs.TaskStatus;

namespace Havoc_API.DTOs.Task;

public class TaskPOST
{
    [Required]
    [MaxLength(25)]
    public string Name { get; set; } = null!;

    [MaxLength(200)]
    public string? Description { get; set; }

    public DateTime? Start { get; set; }

    public DateTime? Deadline { get; set; }

    public TaskStatusPOST TaskStatus { get; set; } = null!;

    [JsonIgnore]
    public int CreatorId { get; set; }

    [Required]
    public int ProjectId { get; set; }

    public virtual ICollection<AssignmentPOST> Assignments { get; set; } = new List<AssignmentPOST>();

    public virtual ICollection<AttachmentPOST> Attachments { get; set; } = new List<AttachmentPOST>();

}
