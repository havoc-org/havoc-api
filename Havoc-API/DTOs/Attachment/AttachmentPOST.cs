using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Havoc_API.DTOs.Attachment;

public class AttachmentPOST
{
    [JsonIgnore]
    public int UserId { get; set; }

    [Required]
    public string FileLink { get; set; } = null!;
}