using Havoc_API.DTOs.Task;
using Havoc_API.DTOs.TaskStatus;
using Havoc_API.Exceptions;
using Havoc_API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace Havoc_API.Controllers;

[Authorize]
[Route("api/tasks")]
[ApiController]
public class TaskController : ControllerBase
{
    private readonly ITaskService _taskService;
    private readonly IUserService _userService;
    private readonly IParticipationService _participationService;

    public TaskController(
        ITaskService taskService,
        IUserService userService,
        IParticipationService participationService)
    {
        _taskService = taskService;
        _userService = userService;
        _participationService = participationService;
    }

    [HttpGet("byId/{taskId}")]
    public async Task<ActionResult> GetTaskAsync(int taskId)
    {
        try
        {
            var task = await _taskService.GetTaskByIdAsync(taskId);
            return Ok(new { task });
        }
        catch (NotFoundException ex)
        {
            return NotFound(new { ex.Message });
        }
        catch (DbUpdateException ex)
        {
            return StatusCode(500, new { ex.Message });
        }
        catch (SqlException ex)
        {
            return StatusCode(500, new { ex.Message });
        }
    }
    [HttpGet("{projectId}")]
    public async Task<ActionResult> GetTasksByProjectIdAsync(int projectId)
    {
        try
        {
            var userId = _userService.GetUserId(Request);
            await _participationService.GetUserRoleInProjectAsync(userId, projectId);

            var tasks = await _taskService.GetTasksByProjectIdAsync(projectId);
            var statuses = await _taskService.GetAllTaskStatusesAsync();
            return Ok(new { statuses, tasks });
        }
        catch (NotFoundException ex)
        {
            return NotFound(new { ex.Message });
        }
        catch (DataAccessException ex)
        {
            return StatusCode(500, new { ex.Message });
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(new { ex.Message });
        }
    }

    [HttpPost]
    public async Task<ActionResult> AddTaskAsync(TaskPOST task)
    {
        try
        {
            var creatorId = _userService.GetUserId(Request);
            task.CreatorId = creatorId;

            var role = await _participationService.GetUserRoleInProjectAsync(creatorId, task.ProjectId);
            if (!role.CanCreateTask())
                return Unauthorized(new { Message = "You have no permission to create tasks" });

            var result = await _taskService.AddTaskAsync(task);
            return Ok(new { TaskId = result });
        }
        catch (NotFoundException ex)
        {
            return NotFound(new { ex.Message });
        }
        catch (DomainException ex)
        {
            return BadRequest(new { ex.Message });
        }
        catch (DataAccessException ex)
        {
            return StatusCode(500, new { ex.Message });
        }
    }

    [HttpDelete("{taskId}")]
    public async Task<ActionResult> DeleteTaskByIdAsync(int taskId)
    {
        var userId = _userService.GetUserId(Request);
        var task = await _taskService.GetTaskByIdAsync(taskId);

        var role = await _participationService.GetUserRoleInProjectAsync(userId, task.ProjectId);
        if (!role.CanDeleteTask())
            return Unauthorized(new { Message = "You have no permission to delete tasks" });

        var result = await _taskService.DeleteTaskByIdAsync(taskId);
        return Ok(new { AffectedRows = result });
    }

    [HttpPatch]
    public async Task<ActionResult> UpdateTaskAsync(TaskPATCH taskUpdate)
    {
        var userId = _userService.GetUserId(Request);
        var task = await _taskService.GetTaskByIdAsync(taskUpdate.TaskId);

        var role = await _participationService.GetUserRoleInProjectAsync(userId, task.ProjectId);
        if (!role.CanEditTask())
            return Unauthorized(new { Message = "You have no permission to edit tasks" });

        var result = await _taskService.UpdateTaskAsync(taskUpdate);
        return Ok(new { AffectedRows = result });
    }

    [HttpPatch("updateStatus")]
    public async Task<ActionResult> UpdateStatusByIdAsync(TaskStatusPATCH taskStatus)
    {
        var userId = _userService.GetUserId(Request);
        var task = await _taskService.GetTaskByIdAsync(taskStatus.TaskId);

        var role = await _participationService.GetUserRoleInProjectAsync(userId, task.ProjectId);
        if (!role.CanEditTask())
            return Unauthorized(new { Message = "You have no permission to edit tasks" });

        var result = await _taskService.UpdateTaskStatusAsync(taskStatus);
        return Ok(new { AffectedRows = result });
    }
}