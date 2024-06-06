using SystemOverseer_API.Auth;
using SystemOverseer_API.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Azure;
using Microsoft.AspNetCore.Authorization;

namespace SystemOverseer_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost]
        [Route("login")]
        public async Task<IActionResult> Login([FromBody] LoginModel model)
        {
            var user = await _authService.Login(model);
            if (user != null)
            {
                return Ok(user);
            }

            return Unauthorized();
        }

        [HttpPost]
        [Route("register")]
        public async Task<IActionResult> Register([FromBody] RegisterModel model)
        {
            AuthResponse authResponse = await _authService.Register(model);

            if (authResponse.Status == "Error")
            {
                return BadRequest(authResponse);
            }

            return Ok(authResponse);
        }

        [Authorize(Roles = UserRoles.Admin)]
        [HttpPost]
        [Route("register-admin")]
        public async Task<IActionResult> RegisterAdmin([FromBody] RegisterModel model)
        {
            AuthResponse authResponse = await _authService.RegisterAdmin(model);

            if (authResponse.Status == "Error")
            {
                return BadRequest(authResponse);
            }

            return Ok(authResponse);
        }

        [HttpPost]
        [Route("confirm-email")]
        public async Task<IActionResult> ConfirmEmail([FromBody] string username)
        {
            AuthResponse authResponse = await _authService.ConfirmEmail(username);

            if (authResponse.Status == "Error")
            {
                return BadRequest(authResponse);
            }

            return Ok(authResponse);
        }
    }
}
