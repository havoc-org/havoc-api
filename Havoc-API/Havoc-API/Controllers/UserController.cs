using Havoc_API.DTOs.User;
using Havoc_API.Exceptions;
using Havoc_API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;

namespace Havoc_API.Controllers;

[Authorize]
[Route("api/users")]
[ApiController]

public class UserController : ControllerBase
{

    private readonly IUserService _userService;

    public UserController(IUserService userService)
    { _userService = userService; }

    [HttpGet("byId/{userId}")]
    public async Task<IActionResult> GetUserAsync(int userId)
    {
        try
        {
            var user = await _userService.GetUserGETByIdAsync(userId);
            return Ok(new { user });

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
    public async Task<ActionResult> UpdateUserAsync(UserPATCH userUpdate)
    {
        try
        {
            var result = await _userService.UpdateUserAsync(userUpdate);
            return Ok(new { AffectedRows = result });
        }
        catch (NotFoundException ex)
        {
            return NotFound(new { ex.Message });
        }
        catch (DataAccessException ex)
        {
            return StatusCode(500, new { ex.Message });
        }
    }
}