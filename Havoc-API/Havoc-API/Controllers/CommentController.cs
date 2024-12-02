using Havoc_API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Havoc_API.Controllers;

[Authorize]
[Route("api/projects/{projectId}/tasks/{taskId}/comments")]
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
    public async Task<IActionResult> GetTasksComments(int taskId, int projectId)
    {
        var userId = _userService.GetUserId(Request);
        var role =
            await _participationService
                .GetUserRoleInProjectAsync(userId, projectId);
        return Ok(await _commentService.GetTasksCommentsAsync(taskId, projectId));
    }

    [HttpPost]
    public async Task<IActionResult> AddCommentToTask(CommentPOST comment, int taskId, int projectId)
    {
        var userId = _userService.GetUserId(Request);
        var role =
            await _participationService
                .GetUserRoleInProjectAsync(userId, projectId);
        if (!role.CanEditTask())
            return Unauthorized("You have no permission to edit task");
        var newComment = await _commentService.AddCommentAsync(comment, userId, taskId, projectId);
        return Created($"api/tasks/{taskId}/comments/{newComment.CommentId}", newComment);
    }

    [HttpDelete("commentId")]
    public async Task<IActionResult> DeleteComment(int commentId, int projectId)
    {
        var userId = _userService.GetUserId(Request);
        var role =
            await _participationService
                .GetUserRoleInProjectAsync(userId, projectId);
        if (!role.CanEditTask())
            return Unauthorized("You have no permission to edit task");
        await _commentService.DeleteCommentAsync(commentId, projectId);
        return NoContent();
    }
}