using CRM_AutoFlow.Application.DTOs;
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

        [HttpPost ("registration")]
        public async Task<IActionResult> RegistrationUser([FromBody] UserDTO user)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState); // автоматическая валидация
            var result = await _userService.AddUser(user);
            if (!result.Success)
                return BadRequest(new {Error = result.Error });
            return Ok(result.Data);
        }
        [HttpPost("signin")]
        public async Task<IActionResult> UserSignIn([FromForm] string phoneNumber,  [FromForm]string password)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var result = await _userService.SignInUser(phoneNumber, password);
            if (!result.Success)
                return Unauthorized(new { Error = result.Error});
            return Ok(new { access_token = result.Data});
        }
    }
}
