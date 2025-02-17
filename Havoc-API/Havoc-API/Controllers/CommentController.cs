using Havoc_API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Havoc_API.Controllers;

[Authorize]
[Route("api/projects/{projectId}/tasks/{taskId}/comments")]
[Route("api/tasks/{taskId}/comments")]
[ApiController]
public class CommentController : ControllerBase
{
    private readonly ICommentService _commentService;
    private readonly IUserService _userService;
    private readonly IParticipationService _participationService;
    public CommentController(
        ICommentService commentService,
        IUserService userService,
        IParticipationService participationService)
    {
        _commentService = commentService;
        _userService = userService;
        _participationService = participationService;
    }

    [HttpGet]
    public async Task<IActionResult> GetTasksComments(int taskId)
    {
        return Ok(await _commentService.GetTasksCommentsAsync(taskId));
    }

    [HttpPost]
    public async Task<IActionResult> AddCommentToTask(CommentPOST comment)
    {

        var userId = _userService.GetUserId(Request);
        var role =
            await _participationService
                .GetUserRoleInProjectAsync(userId, comment.projectId);
        if (!role.CanEditTask())
            return Unauthorized(new { message = "You have no permission to edit task" });
        var newComment = await _commentService.AddCommentAsync(comment, userId, comment.taskId);
        return Created($"api/tasks/{comment.taskId}/comments/{newComment.CommentId}", newComment);
    }

    [HttpDelete("commentId")]
    public async Task<IActionResult> DeleteComment(int commentId, int projectId)
    {
        var userId = _userService.GetUserId(Request);
        var role =
            await _participationService
                .GetUserRoleInProjectAsync(userId, projectId);
        if (!role.CanEditTask())
            return Unauthorized(new { message = "You have no permission to edit task" });
        await _commentService.DeleteCommentAsync(commentId, projectId);
        return NoContent();
    }
}