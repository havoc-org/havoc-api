using Azure.Core;
using Havoc_API.DTOs.Tokens;
using Havoc_API.DTOs.User;
using Havoc_API.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Havoc_API.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly ITokenService _tokenService;
        private readonly IUserService _userService;

        public AuthController(ITokenService tokenService, IUserService userService)
        {
            _tokenService = tokenService;
            _userService = userService;
        }
        [HttpPost("register")]
        public async Task<ActionResult> registerUser(UserPOST user)
        {
            if (await _userService.addUser(user))
                return Ok("User registered successfully!");
            return BadRequest("User with entered email already exists");

        }

        [HttpPost("login")]
        public async Task<ActionResult> loginUser(UserLogin user)
        {
            try
            {
                UserToken userToken = await _userService.verifyUser(user);
                var accessToken = _tokenService.GenerateAccessToken(userToken);

                var refreshToken = _tokenService.GenerateRefreshToken(userToken.Id); // Если нужно, можешь сгенерировать новый refresh token

                var AccessCookieOptions = new CookieOptions
                {
                    HttpOnly = true, //Если true - Ограничивает доступ к кукам только через HTTP, предотвращает доступ к кукам из JavaScript
                    Secure = true, // Устанавливает куку только по HTTPS (рекомендуется в продакшене)
                    SameSite = SameSiteMode.Lax, // Политика SameSite для предотвращения CSRF-атак
                    Expires = DateTime.UtcNow.AddHours(1) // Время жизни куки
                };
                var RefreshCookieOptions = new CookieOptions
                {
                    HttpOnly = true, //Если true - Ограничивает доступ к кукам только через HTTP, предотвращает доступ к кукам из JavaScript
                    Secure = true, // Устанавливает куку только по HTTPS (рекомендуется в продакшене)
                    SameSite = SameSiteMode.Lax, // Политика SameSite для предотвращения CSRF-атак
                    Expires = DateTime.UtcNow.AddDays(3) // Время жизни куки
                };

                Response.Cookies.Append("AuthToken", accessToken, AccessCookieOptions);
                Response.Cookies.Append("RefreshToken", refreshToken, RefreshCookieOptions);

                return Ok(new { accessToken });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpPost("refreshToken")]
        public ActionResult RefreshToken()
        {
            var oldToken = Request.Cookies["RefreshToken"];
            if (oldToken == null)
                return Unauthorized();
            var principal = _tokenService.ValidateRefreshToken(oldToken);
            
            if (principal == null)
                return Unauthorized("Invalid refresh token");
            
            var userIdClaim = principal.Claims.FirstOrDefault(claim => claim.Type == "UserId");

            if (userIdClaim == null)
                return BadRequest(); // Возвращаем ID пользователя

            
            int.TryParse(userIdClaim.Value, out int userId);
            var user = _userService.getUserById(userId).Result;
            var newAccessToken = _tokenService.GenerateAccessToken(new UserToken(user.UserId, user.FirstName, user.LastName, user.Email));
            var newRefreshToken = _tokenService.GenerateRefreshToken(userId); // Если нужно, можешь сгенерировать новый refresh token

            var AccessCookieOptions = new CookieOptions
            {
                HttpOnly = true, //Если true - Ограничивает доступ к кукам только через HTTP, предотвращает доступ к кукам из JavaScript
                Secure = true, // Устанавливает куку только по HTTPS (рекомендуется в продакшене)
                SameSite = SameSiteMode.Lax, // Политика SameSite для предотвращения CSRF-атак
                Expires = DateTime.UtcNow.AddHours(1) // Время жизни куки
            };

            var RefreshCookieOptions = new CookieOptions
            {
                HttpOnly = true, //Если true - Ограничивает доступ к кукам только через HTTP, предотвращает доступ к кукам из JavaScript
                Secure = true, // Устанавливает куку только по HTTPS (рекомендуется в продакшене)
                SameSite = SameSiteMode.Lax, // Политика SameSite для предотвращения CSRF-атак
                Expires = DateTime.UtcNow.AddDays(3) // Время жизни куки
            };

            // Токен валиден, генерируем новый access token
            

            Response.Cookies.Append("AuthToken", newAccessToken, AccessCookieOptions);
            Response.Cookies.Append("RefreshToken", newRefreshToken, RefreshCookieOptions);

            return Ok(new { accessToken = newAccessToken, refreshToken = newRefreshToken });
        }


        [HttpPost("logout")]
        public IActionResult LogoutUser()
        {

            Response.Cookies.Delete("AuthToken");
            Response.Cookies.Delete("RefreshToken");
            // Возврат ответа, например, с сообщением об успешном выходе
            return Ok(new { message = "User logged out successfully" });
        }
    }
}
