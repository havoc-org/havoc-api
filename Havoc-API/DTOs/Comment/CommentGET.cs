using Havoc_API.DTOs.Task;
using Havoc_API.DTOs.User;

namespace Havoc_API.DTOs.Comment;

public class CommentGET
{
    public int CommentId { get; set; }

    // public int TaskId { get; set; }

    // public int UserId { get; set; }

    public string Content { get; set; } = null!;

    public DateTime CommentDate { get; set; }

    public virtual TaskGET Task { get; set; } = null!;

    public virtual UserGET User { get; set; } = null!;

    public CommentGET(int commentId, string content, DateTime commentDate)
    {
        CommentId = commentId;
        Content = content;
        CommentDate = commentDate;
    }
}