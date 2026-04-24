using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Telemachus.Business.Interfaces.Authentication;
using Telemachus.Models;

namespace Telemachus.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        private readonly IAuthenticationService _authenticationService;

        public LoginController(IAuthenticationService authenticationService)
        {
            _authenticationService = authenticationService;
        }

        [HttpPost("token")]
        public async Task<IActionResult> Login([FromBody] LoginViewModel login)
        {
            var loginResponse = await _authenticationService.AuthenticateAsync(login.UserName, login.Password, login.Passcode, login.Comment);

            if (string.IsNullOrEmpty(loginResponse.Token))
            {
                return NotFound();
            }
            return Ok(loginResponse);
        }

        [HttpDelete("passcode")]
        public async Task<IActionResult> ResetPasscode([FromQuery] string passcode)
        {
            try
            {
                await _authenticationService.ResetPasscode(passcode);
            }
            catch (CustomException e)
            {
                return BadRequest(e.Message);
            }
            catch (Exception)
            {
                return BadRequest();
            }

            return NoContent();
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpGet("user")]
        public async Task<IActionResult> GetUserDetails()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            var loginResponse = await _authenticationService.GetUserDetails(userId);

            if (loginResponse == null)
            {
                return Unauthorized();
            }

            return Ok(loginResponse);
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpPost("switch/{id}")]
        public async Task<IActionResult> Switch(string id)
        {
            var loginResponse = await _authenticationService.SwitchAsync(id);

            if (string.IsNullOrEmpty(loginResponse.Token))
            {
                return NotFound();
            }
            return Ok(loginResponse);
        }

        [HttpGet("bootstrap")]
        public async Task<IActionResult> GetBootstrapData()
        {
            var data = await _authenticationService.GetBootstrapData();

            return Ok(data);
        }

        [HttpPost("hash")]
        public IActionResult HashPassword([FromBody] string password)
        {
            if (string.IsNullOrEmpty(password))
                return BadRequest("Password is required.");

            var hasher = new PasswordHasher<object>();
            var hash = hasher.HashPassword(null, password);
            return Ok(hash);
        }
    }
}
