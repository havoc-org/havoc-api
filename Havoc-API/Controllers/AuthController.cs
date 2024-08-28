using Havoc_API.DTOs.Tokens;
using Havoc_API.DTOs.User;
using Havoc_API.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

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
                var token = _tokenService.GenerateToken(userToken);

                var cookieOptions = new CookieOptions
                {
                    HttpOnly = false, //Если true - Ограничивает доступ к кукам только через HTTP, предотвращает доступ к кукам из JavaScript
                    Secure = true, // Устанавливает куку только по HTTPS (рекомендуется в продакшене)
                    SameSite = SameSiteMode.Lax, // Политика SameSite для предотвращения CSRF-атак
                    Expires = DateTime.UtcNow.AddHours(1) // Время жизни куки
                };

                Response.Cookies.Append("AuthToken", token, cookieOptions);

                return Ok(new { token });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        [HttpPost("logout")]
        public IActionResult LogoutUser()
        {

            Response.Cookies.Delete("AuthToken");

            // Возврат ответа, например, с сообщением об успешном выходе
            return Ok(new { message = "User logged out successfully" });
        }
    }
}
