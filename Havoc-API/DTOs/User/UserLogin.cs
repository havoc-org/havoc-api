using System.ComponentModel.DataAnnotations;

namespace Havoc_API.DTOs.User
{
    public class UserLogin
    {
        [Required]
        public string Email { get;  set; } = null!;
        [Required]
        public string Password { get;  set; } = null!;
        
    }
}
