using System.ComponentModel.DataAnnotations;

namespace Havoc_API.DTOs.ProjectStatus
{
    public class ProjectStatusPOST
    {
        [Required][MaxLength(20)] 
        public string Name { get; set; } = null!;

    }
}
