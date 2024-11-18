using Havoc_API.DTOs.Task;
using Havoc_API.DTOs.TaskStatus;
using Havoc_API.Exceptions;
using Havoc_API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using WarehouseApp2.Exceptions;

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

    [HttpGet("{projectId}")]
    public async Task<ActionResult> GetTasksByProjectIdAsync(int projectId)
    {
        try
        {
            var userId = _userService.GetUserId();
            await _participationService.GetUserRoleInProjectAsync(userId, projectId);

            var result = await _taskService.GetTasksByProjectIdAsync(projectId);
            return Ok(result);
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

    [HttpPost]
    public async Task<ActionResult> AddTaskAsync(TaskPOST task)
    {
        try
        {
            var creatorId = _userService.GetUserId();
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
        catch (DbUpdateException ex)
        {
            return StatusCode(500, new { ex.Message });
        }
        catch (SqlException ex)
        {
            return StatusCode(500, new { ex.Message });
        }
    }

    [HttpDelete("{taskId}")]
    public async Task<ActionResult> DeleteTaskByIdAsync(int taskId)
    {
        try
        {
            var userId = _userService.GetUserId();
            var task = await _taskService.GetTaskByIdAsync(taskId);

            var role = await _participationService.GetUserRoleInProjectAsync(userId, task.ProjectId);
            if (!role.CanDeleteTask())
                return Unauthorized(new { Message = "You have no permission to delete tasks" });

            var result = await _taskService.DeleteTaskByIdAsync(taskId);
            return Ok(new { AffectedRows = result });
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

    [HttpPatch]
    public async Task<ActionResult> UpdateTaskAsync(TaskPATCH taskUpdate)
    {
        try
        {
            var userId = _userService.GetUserId();
            var task = await _taskService.GetTaskByIdAsync(taskUpdate.TaskId);

            var role = await _participationService.GetUserRoleInProjectAsync(userId, task.ProjectId);
            if (!role.CanEditTask())
                return Unauthorized(new { Message = "You have no permission to edit tasks" });

            var result = await _taskService.UpdateTaskAsync(taskUpdate);
            return Ok(new { AffectedRows = result });
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

    [HttpPatch("{taskId}/updateStatus")]
    public async Task<ActionResult> UpdateStatusByIdAsync(int taskId, TaskStatusPATCH taskStatus)
    {
        try
        {
            var userId = _userService.GetUserId();
            var task = await _taskService.GetTaskByIdAsync(taskId);

            var role = await _participationService.GetUserRoleInProjectAsync(userId, task.ProjectId);
            if (!role.CanEditTask())
                return Unauthorized(new { Message = "You have no permission to edit tasks" });

            var result = await _taskService.UpdateStatusByIdAsync(taskId, taskStatus);
            return Ok(new { AffectedRows = result });
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
}