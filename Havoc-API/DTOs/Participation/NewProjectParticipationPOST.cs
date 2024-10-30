using Havoc_API.DTOs.Role;
using System.ComponentModel.DataAnnotations;

namespace Havoc_API.DTOs.Participation
{
    public class NewProjectParticipationPOST
    {
        [Required][MaxLength(100)]
        [RegularExpression(@"^[^@\s]+@[^@\s]+\.[^@\s]+$", ErrorMessage = "The Email field is not a valid e-mail address.")]
        public string Email  { get; private set; } = null!;
        public string Role { get; private set; }

        private NewProjectParticipationPOST() { }
        public NewProjectParticipationPOST(string email, string role)
        {

            Email = email;
            Role = role;
            
        }

    }
}
