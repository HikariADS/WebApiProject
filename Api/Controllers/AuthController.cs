using Microsoft.AspNetCore.Mvc;
using WebApiProject.Domain.Entities;
using WebApiProject.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace WebApiProject.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly AppDbContext _context;
        public AuthController(AppDbContext context)
        {
            _context = context;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(string username, string password)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.UserName == username);
            if (user == null) return Unauthorized("Invalid username");
            // giả lập đăng nhập: sau này bạn có thể thay bằng JWT
            return Ok(new { Message = "Login success", User = user });
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(User user)
        {
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return Ok(user);
        }
    }
}
