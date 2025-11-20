using Microsoft.AspNetCore.Mvc;
using WebApiProject.Application.IServices;
using WebApiProject.Application.DTOs.Auth;
using Microsoft.AspNetCore.Authorization;
namespace WebApiProject.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto dto)
        {
           if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var response = await _authService.LoginAsync(dto);
            if (response == null)
            {
                return Unauthorized("Invalid credentials");
            }
            return Ok(response);
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto dto)

        {
            if (!ModelState.IsValid )
            {
                return BadRequest(ModelState);
            }
            var (success, errors) = await _authService.RegisterAsync(dto);
            if (!success)
            {
                return BadRequest(new {errors});
            }
            return Ok(new {Message = "Registration successful"});
        }
        [Authorize(Roles = "Admin")]
        [HttpPost("change-role")]
        public async Task<IActionResult> ChangeRole(ChangeRoleDto dto)
        {
            var result = await _authService.ChangeRoleAsync(dto);
            if (!result.Success)
                return BadRequest(new { errors = result.Errors });

            return Ok(new { Message = "Role updated" });
        }

    }
}
