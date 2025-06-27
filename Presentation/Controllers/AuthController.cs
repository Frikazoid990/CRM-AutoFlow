using CRM_AutoFlow.Application.DTOs;
using CRM_AutoFlow.Domain.Interfaces;
using Form_Registration_App.Services;
using FormRegJWTAndDB.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Models;

namespace FormRegJWTAndDB.Controllers
{
    [AllowAnonymous]
    [ApiController]
    [Route("auth")]
    public class AuthController : Controller
    {
        private readonly UserService _userService;
        private readonly ITokenService _tokenService;

        public AuthController(UserService userService, ITokenService tokenService)
        {
            _userService = userService;
            _tokenService = tokenService;
        }

        [HttpPost("logout")]
        public IActionResult Logout()
        {
            Response.Cookies.Append("access_token", string.Empty, new CookieOptions
            {
                HttpOnly = false,
                Secure = true, // Убедитесь, что это соответствует вашей конфигурации
                SameSite = SameSiteMode.None, // или Strict/Lax — зависит от настроек
                Expires = DateTimeOffset.UtcNow.AddHours(-1), // прошедшая дата для удаления
                Path = "/" // важно, чтобы совпадало с путём, где была установлена кука
            });

            return Ok(new { message = "Logged out successfully" });
        }


        [HttpPost ("registration")]
        public async Task<IActionResult> RegistrationUser([FromBody] UserDTO user)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState); // автоматическая валидация
            var result = await _userService.AddUser(user);
            if (!result.Success)
                return BadRequest(new {Error = result.Error });
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result1 = await _userService.SignInUser(user.PhoneNumber, user.Password);
            if (!result1.Success)
                return Unauthorized(new { Error = result.Error });

            Response.Cookies.Append("access_token", result1.Data, new CookieOptions
            {
                Secure = false,   // Передавать только по HTTPS (в продакшене)
                SameSite = SameSiteMode.Strict, // Защита от CSRF
                Expires = DateTimeOffset.UtcNow.AddDays(7), // Срок действия
                 Path = "/" // Это ключевое изменение - куки будут доступны на всех путях
            });
            return Ok(new { message = "Authentication successful" });
        }
        [HttpPost("signin")]
        public async Task<IActionResult> UserSignIn([FromForm] string phoneNumber,  [FromForm]string password)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var result = await _userService.SignInUser(phoneNumber, password);
            if (!result.Success)
                return Unauthorized(new { Error = result.Error});

            Response.Cookies.Append("access_token", result.Data, new CookieOptions
            {
                HttpOnly = false,
                Secure = true,   // Передавать только по HTTPS (в продакшене)
                Expires = DateTimeOffset.UtcNow.AddDays(7), // Срок действия
                SameSite = SameSiteMode.None,
                Domain = "localhost",
                //Path = "/" // Это ключевое изменение - куки будут доступны на всех путях
            });
            return Ok(new { message = "Authentication successful" });

        }
    }
}
