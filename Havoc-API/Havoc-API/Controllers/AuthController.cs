using Azure.Core;
using Havoc_API.DTOs.Tokens;
using Havoc_API.DTOs.User;
using Havoc_API.Exceptions;
using Havoc_API.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Havoc_API.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly ITokenService _tokenService;
        private readonly IUserService _userService;

        public AuthController(ITokenService tokenService, IUserService userService, IConfiguration configuration)
        {
            _tokenService = tokenService;
            _userService = userService;
        }

        [HttpPost("register")]
        public async Task<ActionResult> RegisterUserAsync(UserPOST user)
        {
            try
            {
                if (await _userService.AddUserAsync(user))
                    return Ok(new { message = "User registered successfully!" });
                return BadRequest(new { message = "User with entered email already exists" });
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

        [HttpPost("login")]
        public async Task<ActionResult> LoginUserAsync(UserLogin user)
        {
            try
            {
                UserToken userToken = await _userService.VerifyUserAsync(user);
                var accessToken = _tokenService.GenerateAccessToken(userToken);

                var refreshToken = _tokenService.GenerateRefreshToken(userToken.Id); // Если нужно, можешь сгенерировать новый refresh token

                var RefreshCookieOptions = new CookieOptions
                {
                    HttpOnly = true, //Если true - Ограничивает доступ к кукам только через HTTP, предотвращает доступ к кукам из JavaScript
                    Secure = true, // Устанавливает куку только по HTTPS (рекомендуется в продакшене)
                    SameSite = SameSiteMode.Lax, // Политика SameSite для предотвращения CSRF-атак
                    Expires = DateTime.UtcNow.AddDays(3) // Время жизни куки
                };

                Response.Cookies.Append("RefreshToken", refreshToken, RefreshCookieOptions);
                //Response.Cookies.Append("UserId", userToken.Id.ToString(), RefreshCookieOptions);
                return Ok(new { accessToken, userId = userToken.Id, email = userToken.Email });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("refresh")]
        public ActionResult RefreshToken()
        {
            try
            {
                var oldToken = Request.Cookies["RefreshToken"];
                if (oldToken == null)
                    return Unauthorized(new { message = "You dont have a refresh token" });
                var principal = _tokenService.ValidateRefreshToken(oldToken);

                if (principal == null)
                    return Unauthorized(new { message = "Invalid refresh token" });

                var userIdClaim = principal.Claims.FirstOrDefault(claim => claim.Type == "UserId");

                if (userIdClaim == null)
                    return BadRequest(); // Возвращаем ID пользователя


                _ = int.TryParse(userIdClaim.Value, out int userId);
                var user = _userService.GetUserByIdAsync(userId).Result;
                var newAccessToken = _tokenService.GenerateAccessToken(new UserToken(user.UserId, user.FirstName, user.LastName, user.Email));
                var newRefreshToken = _tokenService.GenerateRefreshToken(userId); // Если нужно, можешь сгенерировать новый refresh token


                var RefreshCookieOptions = new CookieOptions
                {
                    HttpOnly = true, //Если true - Ограничивает доступ к кукам только через HTTP, предотвращает доступ к кукам из JavaScript
                    Secure = true, // Устанавливает куку только по HTTPS (рекомендуется в продакшене)
                    SameSite = SameSiteMode.Lax, // Политика SameSite для предотвращения CSRF-атак
                    Expires = DateTime.UtcNow.AddDays(3) // Время жизни куки
                };

                Response.Cookies.Append("RefreshToken", newRefreshToken, RefreshCookieOptions);
                return Ok(new { accessToken = newAccessToken, userId = user.UserId, email = user.Email });
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


        [HttpPost("logout")]
        public IActionResult LogoutUser()
        {
            try
            {
                Response.Cookies.Delete("RefreshToken");
                // Возврат ответа, например, с сообщением об успешном выходе
                return Ok(new { message = "User logged out successfully" });
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
    }
}
