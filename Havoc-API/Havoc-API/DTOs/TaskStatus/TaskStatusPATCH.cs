using System.ComponentModel.DataAnnotations;

namespace Havoc_API.DTOs.TaskStatus;

public class TaskStatusPATCH
{
    [MaxLength(20)] 
    public string Name { get; set; } = null!;
}