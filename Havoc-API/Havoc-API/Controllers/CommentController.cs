using Havoc_API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Havoc_API.Controllers;

[Authorize]
[Route("api/tasks/{taskId}/comments")]
[ApiController]
public class CommentController : ControllerBase
{
    private readonly ICommentService _commentService;
    private readonly IUserService _userService;
    public CommentController(ICommentService commentService, IUserService userService)
    {
        _commentService = commentService;
        _userService = userService;
    }

    [HttpGet]
    public async Task<IActionResult> GetTasksComments(int taskId)
    {
        return Ok(await _commentService.GetTasksCommentsAsync(taskId));
    }

    [HttpPost]
    public async Task<IActionResult> AddCommentToTask(CommentPOST comment, int taskId)
    {
        var userId = _userService.GetUserId(Request);
        var newComment = await _commentService.AddCommentAsync(comment, userId, taskId);
        return Created($"api/tasks/{taskId}/comments/{newComment.CommentId}", newComment);
    }

    [HttpDelete("commentId")]
    public async Task<IActionResult> DeleteComment(int commentId, int taskId)
    {
        await _commentService.DeleteCommentAsync(commentId);
        return NoContent();
    }
}