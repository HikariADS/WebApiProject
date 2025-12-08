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

        // Helper lấy Role và UnitId từ claim
        private string? GetCurrentUserRole() => User.Claims.FirstOrDefault(x => x.Type == System.Security.Claims.ClaimTypes.Role)?.Value;
        private string? GetCurrentUserUnitId() => User.Claims.FirstOrDefault(x => x.Type == "unitid")?.Value;

        // GET ALL USERS - chỉ Admin xem all, Manager chỉ xem cùng Unit
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            string currentRole = GetCurrentUserRole() ?? "";
            string? unitId = GetCurrentUserUnitId();
            var query = _userManager.Users.AsQueryable();
            if(currentRole == "Manager" && unitId != null)
            {
                query = query.Where(u => u.UnitId == unitId);
            }
            // User chỉ xem được info bản thân, không được xem all
            else if (currentRole == "User")
            {
                var userId = _userManager.GetUserId(User);
                query = query.Where(u => u.Id == userId);
            }
            var userList = await query.ToListAsync();
            return Ok(userList);
        }

        // GET by ID - Manager chỉ xem cùng Unit
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if(user == null) return NotFound();
            string currentRole = GetCurrentUserRole() ?? "";
            string? unitId = GetCurrentUserUnitId();
            if(currentRole == "Manager" && unitId != null && user.UnitId != unitId)
                return Forbid();
            if(currentRole == "User" && _userManager.GetUserId(User) != id)
                return Forbid();
            return Ok(user);
        }

        // CREATE luôn tạo role User, đơn vị từ body
        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] UserCreateDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var user = new User
            {
                UserName = dto.UserName,
                Email = dto.Email,
                UnitId = dto.UnitId
            };
            var result = await _userManager.CreateAsync(user, dto.Password);
            if(!result.Succeeded)
                return BadRequest(result.Errors);
            await _userManager.AddToRoleAsync(user, "User");
            return CreatedAtAction(nameof(Get), new {id = user.Id}, new { user.Id, user.Email });
        }

        // UPDATE thông tin/tăng quyền (Manager chỉ update cùng Unit, User chỉ tự update)
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(string id, [FromBody] UserUpdateDto dto)
        {
            var user = await _userManager.FindByIdAsync(id);
            if(user == null) return NotFound();
            string currentRole = GetCurrentUserRole() ?? "";
            string? unitId = GetCurrentUserUnitId();
            if(currentRole == "Manager" && unitId != null && user.UnitId != unitId)
                return Forbid();
            if(currentRole == "User" && _userManager.GetUserId(User) != id)
                return Forbid();
            user.FullName = dto.Name;
            user.Email = dto.Email;
            user.UnitId = dto.UnitId;
            var result = await _userManager.UpdateAsync(user);
            if(!result.Succeeded) return BadRequest(result.Errors);
            return NoContent();
        }

        // DELETE user - chỉ admin, manager chỉ được xóa cùng unit
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if(user == null) return NotFound();
            string currentRole = GetCurrentUserRole() ?? "";
            string? unitId = GetCurrentUserUnitId();
            if(currentRole == "Manager" && unitId != null && user.UnitId != unitId)
                return Forbid();
            if(currentRole == "User")
                return Forbid();
            var result = await _userManager.DeleteAsync(user);
            if(!result.Succeeded) return BadRequest(result.Errors);
            return NoContent();
        }

        // PHÂN QUYỀN: Admin có thể nâng User -> Manager cho đúng UnitId
        [Authorize(Roles="Admin")]
        [HttpPost("set-manager")]
        public async Task<IActionResult> SetManager([FromBody] string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if(user == null) return NotFound();
            await _userManager.AddToRoleAsync(user, "Manager");
            // tuỳ logic có thể cần update UnitId!
            return Ok(new{ message="Set Manager thành công"});
        }
    }
}
