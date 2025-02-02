using Havoc_API.DTOs.Comment;
using Havoc_API.Models;

public static class CommentFactory
{
    public static Comment Create(User user, Havoc_API.Models.Task task, string content = "Test comment")
    {
        return new Comment(
            content,
            task,
            user
        );
    }
    public static CommentPOST CreatePost(string content = "Test comments")
    {
        return new CommentPOST(content);
    }

    public static CommentGET CreateGet(Comment comment)
    {
        return new CommentGET(
            comment.CommentId,
            comment.Content,
            comment.CommentDate,
            new Havoc_API.DTOs.User.UserGET(
                comment.UserId,
                comment.User.FirstName,
                comment.User.LastName,
                comment.User.Email
            )
        );
    }
}