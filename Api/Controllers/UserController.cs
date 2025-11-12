using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApiProject.Application.DTOs.User;
using WebApiProject.Domain.Entities;
using WebApiProject.Application.IServices;

namespace WebApiProject.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class UserController : ControllerBase
    {
        private readonly UserManager<User> _userManager;
        private readonly IUserService _userService;
        public UserController(UserManager<User> userManager, IUserService userService)
        {
            _userManager = userManager;
            _userService = userService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var user = await _userManager.Users.ToListAsync();
            return Ok(user);
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return NotFound();
            return Ok(user);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] UserCreateDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var user = new User
            {
                UserName = dto.UserName,
                Email = dto.Email,
            };
            var (success, errors) = await _userService.CreateUserAsync(dto); 
            if (!success)
            {
                return BadRequest(errors);
            }
            await _userManager.AddToRoleAsync(user, "User");
            return CreatedAtAction(nameof(Get), new { id = user.Id }, new { Id = user.Id, Email = user.Email });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(string id, [FromBody] UserUpdateDto dto)
        {
            var (success, errors) = await _userService.UpdateUserAsync(id, dto);
            if (!success)
            {
                return BadRequest(new { Errors = errors });
            }
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return NotFound();
            var result = await _userManager.DeleteAsync(user);
            if (!result.Succeeded)
            {
                return BadRequest(result.Errors);
            }
            return NoContent();
        }
    }
}
