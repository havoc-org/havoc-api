using Havoc_API.Models;

public static class AttachmentFactory
{
    public static Attachment Create(User user, Havoc_API.Models.Task task)
    {
        return new Attachment(
            "TestFileLink",
            task,
            user
        );
    }
}