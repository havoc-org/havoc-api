using System.ComponentModel.DataAnnotations;
using Havoc_API.DTOs.TaskStatus;

namespace Havoc_API.DTOs.Task;

public class TaskPATCH
{
    public int TaskId { get; set; }

    [MaxLength(25)]
    public string Name { get; set; } = null!;

    [MaxLength(200)]
    public string? Description { get; set; }

    public DateTime? Start { get; set; }

    public DateTime? Deadline { get; set; }
}