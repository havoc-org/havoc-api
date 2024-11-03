using Havoc_API.DTOs.Task;
using Havoc_API.DTOs.User;

namespace Havoc_API.DTOs.Attachment;

public class AttachmentGET
{
    public int AttachmentId { get; set; }

    public string FileLink { get; set; } = null!;

    public virtual TaskGET Task { get; set; } = null!;

    public virtual UserGET User { get; set; } = null!;

    public AttachmentGET(int attachmentId, string fileLink, UserGET user)
    {
        AttachmentId = attachmentId;
        FileLink = fileLink;
        User = user;
    }
}