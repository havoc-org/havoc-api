using Havoc_API.Data;
using Havoc_API.DTOs.Comment;
using Havoc_API.DTOs.User;
using Havoc_API.Exceptions;
using Havoc_API.Models;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace Havoc_API.Services;

public class CommentService : ICommentService
{
    private readonly IHavocContext _havocContext;
    public CommentService(IHavocContext havocContext)
    {
        _havocContext = havocContext;
    }


    public async Task<IEnumerable<CommentGET>> GetTasksCommentsAsync(int taskId, int projectId)
    {
        try
        {
            if (await _havocContext.Tasks.FirstOrDefaultAsync(t => t.TaskId == taskId && t.ProjectId == projectId) is null)
                throw new NotFoundException("Task or Project doesnt exist");
            return await _havocContext.Comments
                .Include(c => c.User)
                .Include(c => c.Task)
                .Where(c => c.TaskId == taskId && c.Task.ProjectId == projectId)
                .Select
                (
                    c => new CommentGET
                    (
                        c.CommentId,
                        c.Content,
                        c.CommentDate,
                        new UserGET
                        (
                            c.User.UserId,
                            c.User.FirstName,
                            c.User.LastName,
                            c.User.Email
                        )
                    )
                )
                .ToListAsync();
        }
        catch (SqlException e)
        {
            throw new DataAccessException(e.Message);
        }
    }

    public async Task<int> DeleteCommentAsync(int commentId, int projectId)
    {
        try
        {
            var comment = await _havocContext.Comments
                .Include(c => c.Task)
                .FirstOrDefaultAsync(c => c.CommentId == commentId &&
                    c.Task.ProjectId == projectId)
                    ?? throw new NotFoundException("Comment doesn't exist");
            _havocContext.Comments.Remove(comment);
            return await _havocContext.SaveChangesAsync();
        }
        catch (SqlException e)
        {
            throw new DataAccessException(e.Message);
        }
        catch (DbUpdateException e)
        {
            throw new DataAccessException(e.Message);
        }
    }

    public async Task<CommentGET> AddCommentAsync(CommentPOST comment, int userId, int taskId, int projectId)
    {
        try
        {
            var user =
                await _havocContext.Users.FirstOrDefaultAsync(u => u.UserId == userId)
                    ?? throw new NotFoundException("User doesn't exist");
            var task =
                await _havocContext.Tasks
                    .Include(t => t.Project)
                    .FirstOrDefaultAsync(t => t.TaskId == taskId && t.ProjectId == projectId)
                        ?? throw new NotFoundException("Task doesn't exist");
            var newComment = new Comment(comment.Content, task, user);
            await _havocContext.Comments.AddAsync(newComment);
            await _havocContext.SaveChangesAsync();

            return new CommentGET
                    (
                        newComment.CommentId,
                        newComment.Content,
                        newComment.CommentDate,
                        new UserGET
                        (
                            newComment.User.UserId,
                            newComment.User.FirstName,
                            newComment.User.LastName,
                            newComment.User.Email
                        )
                    );
        }
        catch (SqlException e)
        {
            throw new DataAccessException(e.Message);
        }
        catch (DbUpdateException e)
        {
            throw new DataAccessException(e.Message);
        }
    }
}