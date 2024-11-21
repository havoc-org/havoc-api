using System.ComponentModel.DataAnnotations;

namespace Havoc_API.DTOs.Tag;

public class TagPOST
{
    [Required]
    [MaxLength(20)]
    public string Name { get; set; } = null!;

    public string ColorHex { get; set; } = null!;
}