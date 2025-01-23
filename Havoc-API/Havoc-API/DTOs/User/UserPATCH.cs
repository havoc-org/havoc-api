using System.ComponentModel.DataAnnotations;

namespace Havoc_API.DTOs.User;
public class UserPATCH
{
    public int UserId { get; set; }

    [MaxLength(50)]
    public string Name { get; set; } = null!;
    [MaxLength(50)]
    public string SurName { get; set; } = null!;
}
