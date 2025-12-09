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
                
                // Load users để map ManagerName
                var allUsers = await _context.Users.ToListAsync();
                var userDict = allUsers.Where(u => !string.IsNullOrEmpty(u.FullName))
                    .ToDictionary(u => u.Id, u => u.FullName);
                
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
                    ManagerName = s.ManagerId != null && userDict.ContainsKey(s.ManagerId) ? userDict[s.ManagerId] : null
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
        public async Task<IActionResult> Create([FromBody] Storage storage)
        {
            string currentRole = GetCurrentUserRole() ?? "";
            string? unitId = GetCurrentUserUnitId();
            if(currentRole == "User") return Forbid();
            
            // Lấy current user ID
            var currentUser = _context.Users.FirstOrDefault(u => u.UserName == User.Identity.Name);
            if(currentUser == null) return Unauthorized();
            
            storage.UserId = currentUser.Id;
            
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
            
            return Ok(result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] Storage storage)
        {
            string currentRole = GetCurrentUserRole() ?? "";
            string? unitId = GetCurrentUserUnitId();
            if(currentRole == "User") return Forbid();
            
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
            
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var storage = await _context.Storages.FindAsync(id);
            if(storage == null) return NotFound();
            string currentRole = GetCurrentUserRole() ?? "";
            string? unitId = GetCurrentUserUnitId();
            if(currentRole == "User") return Forbid();
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
    }
}
