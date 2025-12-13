using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApiProject.Application.DTOs.User;
using WebApiProject.Application.DTOs.Auth;
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
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IUserService _userService;
        public UserController(UserManager<User> userManager, RoleManager<IdentityRole> roleManager, IUserService userService)
        {
            _userManager = userManager;
            _roleManager = roleManager;
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
            
            // Fetch roles cho mỗi user và trả về cùng với user data
            var usersWithRoles = new List<object>();
            foreach (var user in userList)
            {
                var roles = await _userManager.GetRolesAsync(user);
                usersWithRoles.Add(new
                {
                    user.Id,
                    user.UserName,
                    user.Email,
                    user.FullName,
                    user.PhoneNumber,
                    user.UnitId,
                    user.CreatedDate,
                    user.EmailConfirmed,
                    Roles = roles.ToList(),
                    Role = roles.FirstOrDefault() ?? "User" // Role chính (role đầu tiên)
                });
            }
            
            return Ok(usersWithRoles);
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
            
            // Fetch roles và trả về cùng với user data
            var roles = await _userManager.GetRolesAsync(user);
            var userWithRole = new
            {
                user.Id,
                user.UserName,
                user.Email,
                user.FullName,
                user.PhoneNumber,
                user.UnitId,
                user.CreatedDate,
                user.EmailConfirmed,
                Roles = roles.ToList(),
                Role = roles.FirstOrDefault() ?? "User" // Role chính (role đầu tiên)
            };
            
            return Ok(userWithRole);
        }

        // CREATE - tạo user với role từ DTO (chỉ Admin và Manager mới có thể tạo user)
        [HttpPost]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> Create([FromBody] UserCreateDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            
            // Chỉ Admin mới có thể tạo user với role Admin hoặc Manager
            string currentRole = GetCurrentUserRole() ?? "";
            if ((dto.Role == "Admin" || dto.Role == "Manager") && currentRole != "Admin")
            {
                return Forbid("Chỉ Admin mới có thể tạo user với role Admin hoặc Manager");
            }
            
            var user = new User
            {
                UserName = dto.UserName,
                Email = dto.Email,
                FullName = dto.Name,
                UnitId = dto.UnitId
            };
            var result = await _userManager.CreateAsync(user, dto.Password);
            if(!result.Succeeded)
                return BadRequest(result.Errors);
            
            // Set role từ DTO, mặc định là "User" nếu không có
            var roleToAssign = string.IsNullOrEmpty(dto.Role) ? "User" : dto.Role;
            await _userManager.AddToRoleAsync(user, roleToAssign);
            
            // Trả về user với role
            var roles = await _userManager.GetRolesAsync(user);
            return CreatedAtAction(nameof(Get), new {id = user.Id}, new 
            { 
                user.Id, 
                user.Email,
                user.UserName,
                user.FullName,
                Role = roles.FirstOrDefault() ?? "User",
                Roles = roles.ToList()
            });
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

        // DELETE user - chỉ Admin mới có thể xóa user
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
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

        // PHÂN QUYỀN: Admin có thể thay đổi role của user
        [Authorize(Roles="Admin")]
        [HttpPut("{id}/change-role")]
        public async Task<IActionResult> ChangeRole(string id, [FromBody] ChangeRoleDto dto)
        {
            try
            {
                if (dto == null || string.IsNullOrEmpty(dto.UserId) || string.IsNullOrEmpty(dto.NewRole))
                {
                    return BadRequest(new { message = "Dữ liệu không hợp lệ" });
                }

                if (id != dto.UserId)
                {
                    return BadRequest(new { message = "User ID không khớp" });
                }

                var user = await _userManager.FindByIdAsync(id);
                if(user == null) 
                {
                    return NotFound(new { message = "Không tìm thấy user" });
                }

                // Validate role
                var validRoles = new[] { "User", "Manager", "Admin" };
                if (!validRoles.Contains(dto.NewRole))
                {
                    return BadRequest(new { message = "Role không hợp lệ. Chỉ chấp nhận: User, Manager, Admin" });
                }

                // Đảm bảo role tồn tại trong database
                if (!await _roleManager.RoleExistsAsync(dto.NewRole))
                {
                    // Tạo role nếu chưa tồn tại
                    var createRoleResult = await _roleManager.CreateAsync(new IdentityRole(dto.NewRole));
                    if (!createRoleResult.Succeeded)
                    {
                        return StatusCode(500, new { message = $"Không thể tạo role {dto.NewRole}", errors = createRoleResult.Errors });
                    }
                }

                // Lấy roles hiện tại và xóa tất cả
                var currentRoles = await _userManager.GetRolesAsync(user);
                if (currentRoles != null && currentRoles.Any())
                {
                    var removeResult = await _userManager.RemoveFromRolesAsync(user, currentRoles);
                    if (!removeResult.Succeeded)
                    {
                        return BadRequest(new { message = "Không thể xóa role cũ", errors = removeResult.Errors });
                    }
                }

                // Thêm role mới
                var result = await _userManager.AddToRoleAsync(user, dto.NewRole);
                if (!result.Succeeded)
                {
                    return BadRequest(new { message = "Không thể thêm role mới", errors = result.Errors });
                }

                return Ok(new { message = $"Đã thay đổi role thành {dto.NewRole}" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Lỗi server khi thay đổi role", error = ex.Message });
            }
        }

        // PHÂN QUYỀN: Admin có thể nâng User -> Manager cho đúng UnitId (deprecated, dùng change-role)
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
