using System.ComponentModel.DataAnnotations;

namespace Havoc_API.DTOs.Assignment;

public class AssignmentPOST
{
    [Required]
    public int UserId { get; set; }

    [MaxLength(200)]
    public string? Description { get; set; }
}