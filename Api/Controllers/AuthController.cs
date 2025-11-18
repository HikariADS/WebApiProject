using Microsoft.AspNetCore.Mvc;
using WebApiProject.Domain.Entities;
using WebApiProject.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using WebApiProject.Application.IServices;
using WebApiProject.Application.DTOs.Auth;

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
                return BadRequest(ModelState);
            }
            return Ok(new {Message = "Registration successful"});
        }
    }
}
