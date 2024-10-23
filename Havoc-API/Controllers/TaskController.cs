using Havoc_API.DTOs.Task;
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

    public TaskController(ITaskService taskService, IUserService userService)
    {
        _taskService = taskService;
        _userService = userService;
    }

    [HttpGet("{projectId}")]
    public async Task<ActionResult> GetTasksByProjectIdAsync(int projectId)
    {
        try
        {
            var result = await _taskService.GetTasksByProjectIdAsync(projectId);
            return Ok(result);
        }
        catch (NotFoundException ex)
        {
            return NotFound(ex.Message);
        }
        catch (DbUpdateException ex)
        {
            return StatusCode(500, ex);
        }
        catch (SqlException ex)
        {
            return StatusCode(500, ex);
        }
    }

    [HttpPost]
    public async Task<ActionResult> AddProjectAsync(TaskPOST task)
    {
        try
        {
            var creatorId = _userService.GetUserId(Request);
            task.CreatorId = creatorId;

            var result = await _taskService.AddTaskAsync(task);
            return Ok(result);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(ex.Message);
        }
        catch (NotFoundException ex)
        {
            return  NotFound(ex.Message);
        }
        catch (DomainException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (DbUpdateException ex)
        {
            return StatusCode(500, ex.Message);
        }
        catch (SqlException ex)
        {
            return StatusCode(500, ex.Message);
        }
    }
}