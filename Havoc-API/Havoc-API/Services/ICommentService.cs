using Havoc_API.DTOs.Comment;

namespace Havoc_API.Services;

public interface ICommentService
{
    public Task<IEnumerable<CommentGET>> GetTasksCommentsAsync(int taskId, int projectId);
    public Task<int> DeleteCommentAsync(int commentId, int projectId);
    public Task<CommentGET> AddCommentAsync(CommentPOST comment, int userId, int taskId, int projectId);
}