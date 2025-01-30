using System.ComponentModel.DataAnnotations;

namespace Havoc_API.DTOs.Assignment
{
    public class AssignmentDELETE
    {
        [Required]
        public int UserId { get; set; }
    }
}
