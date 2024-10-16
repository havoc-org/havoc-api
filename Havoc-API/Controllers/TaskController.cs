using Havoc_API.Exceptions;
using Havoc_API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Havoc_API.Controllers;

[Authorize]
[Route("api/tasks")]
[ApiController]
public class TaskController : ControllerBase
{
    private readonly ITaskService _taskService;
    public TaskController(ITaskService taskService)
    {
        _taskService = taskService;
    }

    [HttpGet("{projectId}")]
    public async Task<ActionResult> GetTasksByProjectIdAsync(int projectId)
    {
        try
        {
            return Ok(await _taskService.GetTasksByProjectIdAsync(projectId));
        }
        catch (NotFoundException ex)
        {
            return NotFound(ex.Message);
        }
    }
}