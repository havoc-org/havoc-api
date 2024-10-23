using System.ComponentModel.DataAnnotations;

namespace Havoc_API.DTOs.TaskStatus;

public class TaskStatusPOST
{
    [Required]
    [MaxLength(20)] 
    public string Name { get; set; } = null!;
}