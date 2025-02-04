using Havoc_API.DTOs.Attachment;
using Havoc_API.DTOs.User;
using Havoc_API.Models;

public static class AttachmentFactory
{
    public static Attachment Create(User user, Havoc_API.Models.Task task, string FileLink = "TestFileLink")
    {
        return new Attachment(
            FileLink,
            task,
            user
        );
    }

    public static AttachmentPOST CreatePost(int userId = 0, string fileLink = "TestFileLink")
    {
        return new AttachmentPOST
        {
            UserId = userId,
            FileLink = fileLink
        };
    }
    public static AttachmentGET CreateGet(Attachment attachment)
    {
        return new AttachmentGET(
            attachment.AttachmentId,
            attachment.FileLink,
            new UserGET(
                attachment.UserId,
                attachment.User.FirstName,
                attachment.User.LastName,
                attachment.User.Email
            )
        );
    }
}