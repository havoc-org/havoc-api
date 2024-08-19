using System.ComponentModel.DataAnnotations;

namespace Havoc_API.DTOs.User
{
    public class UserPOST
    {
        [Required]
        public string FirstName { get; set; } = null!;
        [Required]
        public string LastName { get; set; } = null!;
        [Required]
        public string Email { get; set; } = null!;
        [Required]
        public string Password { get; set; } = null!;
    }
}
