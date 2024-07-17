using System.ComponentModel.DataAnnotations;

namespace Havoc_API.Models.DTOs.ProjectStatus
{
    public class ProjectStatusPOST
    {
        [Required][MaxLength(20)] public string Name { get; set; } = null!;

    }
}
