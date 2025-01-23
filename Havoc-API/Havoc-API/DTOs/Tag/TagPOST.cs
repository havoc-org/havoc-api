using System.ComponentModel.DataAnnotations;
using Microsoft.Identity.Client;

namespace Havoc_API.DTOs.Tag;

public class TagPOST
{
    [Required]
    [MaxLength(20)]
    public string Name { get; set; } = null!;

    public int TaskId { get; set; } 

    public string ColorHex { get; set; } = null!;
}