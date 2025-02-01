using Havoc_API.Models;

public static class CommentFactory
{
    public static Comment Create(User user, Havoc_API.Models.Task task)
    {
        return new Comment(
            "Test comment",
            task,
            user
        );
    }
}