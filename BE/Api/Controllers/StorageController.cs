using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApiProject.Domain.Entities;
using WebApiProject.Infrastructure.Persistence;
using Microsoft.AspNetCore.Authorization;

namespace WebApiProject.Api.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class StorageController : ControllerBase
    {
        private readonly AppDbContext _context;
        public StorageController(AppDbContext context)
        {
            _context = context;
        }

        private string? GetCurrentUserRole() => User.Claims.FirstOrDefault(x => x.Type == System.Security.Claims.ClaimTypes.Role)?.Value;
        private string? GetCurrentUserUnitId() => User.Claims.FirstOrDefault(x => x.Type == "unitid")?.Value;

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                string currentRole = GetCurrentUserRole() ?? "";
                string? unitId = GetCurrentUserUnitId();
                
                var query = _context.Storages
                    .Include(s => s.Product)
                    .Include(s => s.StorageType)
                    .Include(s => s.User)
                    .AsQueryable();
                
                string? currentUserId = null;
                if(currentRole == "Manager" && unitId != null)
                {
                    var userName = User.Identity?.Name;
                    if (!string.IsNullOrEmpty(userName))
                    {
                        var currentUser = await _context.Users.FirstOrDefaultAsync(u => u.UserName == userName);
                        currentUserId = currentUser?.Id;
                    }
                    
                    if (currentUserId != null)
                    {
                        query = query.Where(s => s.BelongToUnitId == unitId || s.ManagerId == currentUserId);
                    }
                    else
                    {
                        query = query.Where(s => s.BelongToUnitId == unitId);
                    }
                }
                else if(currentRole == "User")
                {
                    return Forbid();
                }
                
                // Load users để map ManagerName - lấy cả UserName nếu không có FullName
                var allUsers = await _context.Users.ToListAsync();
                var userDict = allUsers.ToDictionary(u => u.Id, u => 
                    !string.IsNullOrEmpty(u.FullName) ? u.FullName : u.UserName ?? "Unknown");
                
                var list = await query.Select(s => new
                {
                    s.Id,
                    ProductId = s.ProductId,
                    ProductName = s.Product != null ? s.Product.Name : "",
                    StorageTypeId = s.StorageTypeId,
                    StorageTypeName = s.StorageType != null ? s.StorageType.Name : "",
                    StorageLocation = s.StorageType != null ? s.StorageType.StorageLocation : "",
                    Quantity = s.Quantity,
                    ImportDate = s.ImportDate,
                    ExportDate = s.ExportDate,
                    UserId = s.UserId,
                    UserName = s.User != null ? s.User.UserName : "",
                    BelongToUnitId = s.BelongToUnitId,
                    ManagerId = s.ManagerId
                }).ToListAsync();
                
                // Map ManagerName sau khi query
                var result = list.Select(s => new
                {
                    s.Id,
                    s.ProductId,
                    s.ProductName,
                    s.StorageTypeId,
                    s.StorageTypeName,
                    s.StorageLocation,
                    s.Quantity,
                    s.ImportDate,
                    s.ExportDate,
                    s.UserId,
                    s.UserName,
                    s.BelongToUnitId,
                    s.ManagerId,
                    ManagerName = s.ManagerId != null && userDict.ContainsKey(s.ManagerId) 
                        ? userDict[s.ManagerId] 
                        : (s.ManagerId != null ? "Chưa có thông tin" : null)
                }).ToList();
                
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Lỗi khi tải danh sách kho", error = ex.Message, stackTrace = ex.StackTrace });
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var storage = await _context.Storages
                .Include(s => s.Product)
                .Include(s => s.StorageType)
                .Include(s => s.User)
                .FirstOrDefaultAsync(s => s.Id == id);
                
            if(storage == null) return NotFound();
            
            string currentRole = GetCurrentUserRole() ?? "";
            string? unitId = GetCurrentUserUnitId();
            var currentUserId = _context.Users.FirstOrDefault(u => u.UserName == User.Identity.Name)?.Id;
            
            if(currentRole == "Manager" && unitId != null && storage.BelongToUnitId != unitId && storage.ManagerId != currentUserId)
                return Forbid();
            if(currentRole == "User") return Forbid();
            
            var result = new
            {
                storage.Id,
                ProductId = storage.ProductId,
                ProductName = storage.Product != null ? storage.Product.Name : "",
                StorageTypeId = storage.StorageTypeId,
                StorageTypeName = storage.StorageType != null ? storage.StorageType.Name : "",
                StorageLocation = storage.StorageType != null ? storage.StorageType.StorageLocation : "",
                Quantity = storage.Quantity,
                ImportDate = storage.ImportDate,
                ExportDate = storage.ExportDate,
                UserId = storage.UserId,
                UserName = storage.User != null ? storage.User.UserName : "",
                BelongToUnitId = storage.BelongToUnitId,
                ManagerId = storage.ManagerId
            };
            
            return Ok(result);
        }

        [HttpPost]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> Create([FromBody] Storage storage)
        {
            string currentRole = GetCurrentUserRole() ?? "";
            string? unitId = GetCurrentUserUnitId();
            
            // Lấy current user ID
            var currentUser = _context.Users.FirstOrDefault(u => u.UserName == User.Identity.Name);
            if(currentUser == null) return Unauthorized();
            
            storage.UserId = currentUser.Id;
            
            // Nếu ManagerId không được set, và current user là Manager, set ManagerId = currentUserId
            if (string.IsNullOrEmpty(storage.ManagerId) && currentRole == "Manager")
            {
                storage.ManagerId = currentUser.Id;
            }
            // Nếu ManagerId vẫn null và có manager user, set mặc định
            else if (string.IsNullOrEmpty(storage.ManagerId))
            {
                var defaultManager = await _context.Users
                    .Where(u => _context.UserRoles.Any(ur => ur.UserId == u.Id && 
                        _context.Roles.Any(r => r.Id == ur.RoleId && r.Name == "Manager")))
                    .FirstOrDefaultAsync();
                if (defaultManager != null)
                {
                    storage.ManagerId = defaultManager.Id;
                }
            }
            
            if(currentRole == "Manager" && (storage.BelongToUnitId != unitId)) 
                return Forbid();
            
            _context.Storages.Add(storage);
            await _context.SaveChangesAsync();
            
            // Load related data để trả về
            await _context.Entry(storage).Reference(s => s.Product).LoadAsync();
            await _context.Entry(storage).Reference(s => s.StorageType).LoadAsync();
            
            var result = new
            {
                storage.Id,
                ProductId = storage.ProductId,
                ProductName = storage.Product != null ? storage.Product.Name : "",
                StorageTypeId = storage.StorageTypeId,
                StorageTypeName = storage.StorageType != null ? storage.StorageType.Name : "",
                StorageLocation = storage.StorageType != null ? storage.StorageType.StorageLocation : "",
                Quantity = storage.Quantity,
                ImportDate = storage.ImportDate,
                ExportDate = storage.ExportDate,
                UserId = storage.UserId,
                BelongToUnitId = storage.BelongToUnitId,
                ManagerId = storage.ManagerId
            };
            
            // Load related data để trả về ManagerName
            var allUsers = await _context.Users.ToListAsync();
            var userDict = allUsers.ToDictionary(u => u.Id, u => 
                !string.IsNullOrEmpty(u.FullName) ? u.FullName : u.UserName ?? "Unknown");
            
            var finalResult = new
            {
                result.Id,
                result.ProductId,
                result.ProductName,
                result.StorageTypeId,
                result.StorageTypeName,
                result.StorageLocation,
                result.Quantity,
                result.ImportDate,
                result.ExportDate,
                result.UserId,
                result.BelongToUnitId,
                result.ManagerId,
                ManagerName = result.ManagerId != null && userDict.ContainsKey(result.ManagerId) 
                    ? userDict[result.ManagerId] 
                    : (result.ManagerId != null ? "Chưa có thông tin" : null)
            };
            
            return Ok(finalResult);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> Update(int id, [FromBody] Storage storage)
        {
            string currentRole = GetCurrentUserRole() ?? "";
            string? unitId = GetCurrentUserUnitId();
            
            var origin = await _context.Storages.FindAsync(id);
            if(origin == null) return NotFound();
            
            if(currentRole == "Manager")
            {
                var currentUserId = _context.Users.FirstOrDefault(u => u.UserName == User.Identity.Name)?.Id;
                if(origin.BelongToUnitId != unitId && origin.ManagerId != currentUserId)
                    return Forbid();
            }
            
            if(id != storage.Id) return BadRequest();
            
            // Cập nhật các trường
            origin.ProductId = storage.ProductId;
            origin.StorageTypeId = storage.StorageTypeId;
            origin.Quantity = storage.Quantity;
            origin.ImportDate = storage.ImportDate;
            origin.ExportDate = storage.ExportDate;
            origin.BelongToUnitId = storage.BelongToUnitId;
            origin.ManagerId = storage.ManagerId;
            
            // Nếu ManagerId không được set, và current user là Manager, set ManagerId = currentUserId
            if (string.IsNullOrEmpty(origin.ManagerId))
            {
                if (currentRole == "Manager")
                {
                    var userName = User.Identity?.Name;
                    if (!string.IsNullOrEmpty(userName))
                    {
                        var currentUser = _context.Users.FirstOrDefault(u => u.UserName == userName);
                        if (currentUser != null)
                        {
                            origin.ManagerId = currentUser.Id;
                        }
                    }
                }
                // Nếu vẫn null, set manager mặc định
                if (string.IsNullOrEmpty(origin.ManagerId))
                {
                    var defaultManager = await _context.Users
                        .Where(u => _context.UserRoles.Any(ur => ur.UserId == u.Id && 
                            _context.Roles.Any(r => r.Id == ur.RoleId && r.Name == "Manager")))
                        .FirstOrDefaultAsync();
                    if (defaultManager != null)
                    {
                        origin.ManagerId = defaultManager.Id;
                    }
                }
            }
            
            await _context.SaveChangesAsync();
            
            // Load related data để trả về ManagerName
            await _context.Entry(origin).Reference(s => s.Product).LoadAsync();
            await _context.Entry(origin).Reference(s => s.StorageType).LoadAsync();
            
            var allUsers = await _context.Users.ToListAsync();
            var userDict = allUsers.ToDictionary(u => u.Id, u => 
                !string.IsNullOrEmpty(u.FullName) ? u.FullName : u.UserName ?? "Unknown");
            
            var result = new
            {
                origin.Id,
                ProductId = origin.ProductId,
                ProductName = origin.Product != null ? origin.Product.Name : "",
                StorageTypeId = origin.StorageTypeId,
                StorageTypeName = origin.StorageType != null ? origin.StorageType.Name : "",
                StorageLocation = origin.StorageType != null ? origin.StorageType.StorageLocation : "",
                Quantity = origin.Quantity,
                ImportDate = origin.ImportDate,
                ExportDate = origin.ExportDate,
                UserId = origin.UserId,
                BelongToUnitId = origin.BelongToUnitId,
                ManagerId = origin.ManagerId,
                ManagerName = origin.ManagerId != null && userDict.ContainsKey(origin.ManagerId) 
                    ? userDict[origin.ManagerId] 
                    : (origin.ManagerId != null ? "Chưa có thông tin" : null)
            };
            
            return Ok(result);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> Delete(int id)
        {
            var storage = await _context.Storages.FindAsync(id);
            if(storage == null) return NotFound();
            string currentRole = GetCurrentUserRole() ?? "";
            string? unitId = GetCurrentUserUnitId();
            if(currentRole == "Manager")
            {
                var userName = User.Identity?.Name;
                string? currentUserId = null;
                if (!string.IsNullOrEmpty(userName))
                {
                    var currentUser = await _context.Users.FirstOrDefaultAsync(u => u.UserName == userName);
                    currentUserId = currentUser?.Id;
                }
                if(storage.BelongToUnitId != unitId && storage.ManagerId != currentUserId)
                    return Forbid();
            }
            _context.Storages.Remove(storage);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpPost("update-manager-ids")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateManagerIds()
        {
            try
            {
                // Lấy manager user đầu tiên
                var managerUser = await _context.Users
                    .Where(u => _context.UserRoles.Any(ur => ur.UserId == u.Id && 
                        _context.Roles.Any(r => r.Id == ur.RoleId && r.Name == "Manager")))
                    .FirstOrDefaultAsync();
                
                if (managerUser == null)
                {
                    return BadRequest(new { message = "Không tìm thấy user có role Manager" });
                }
                
                // Cập nhật tất cả storage có ManagerId null
                var storagesToUpdate = await _context.Storages
                    .Where(s => s.ManagerId == null || s.ManagerId == "")
                    .ToListAsync();
                
                foreach (var storage in storagesToUpdate)
                {
                    storage.ManagerId = managerUser.Id;
                }
                
                await _context.SaveChangesAsync();
                
                return Ok(new { 
                    message = $"Đã cập nhật {storagesToUpdate.Count} storage với ManagerId", 
                    managerId = managerUser.Id,
                    managerName = !string.IsNullOrEmpty(managerUser.FullName) ? managerUser.FullName : managerUser.UserName
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Lỗi khi cập nhật ManagerId", error = ex.Message });
            }
        }
    }
}
