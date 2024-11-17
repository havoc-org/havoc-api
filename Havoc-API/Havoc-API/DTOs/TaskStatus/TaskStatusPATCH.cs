using System.ComponentModel.DataAnnotations;

namespace Havoc_API.DTOs.TaskStatus;

public class TaskStatusPATCH
{
    [Required]
    public int TaskId { get; set; }

    [MaxLength(20)] 
    public string Name { get; set; } = null!;
}