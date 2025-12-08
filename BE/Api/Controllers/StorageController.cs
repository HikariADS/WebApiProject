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
            string currentRole = GetCurrentUserRole() ?? "";
            string? unitId = GetCurrentUserUnitId();
            var query = _context.Storages.AsQueryable();
            if(currentRole == "Manager" && unitId != null)
                query = query.Where(s => s.BelongToUnitId == unitId || s.ManagerId == _context.Users.FirstOrDefault(u => u.UserName == User.Identity.Name).Id);
            else if(currentRole == "User")
                return Forbid();
            var list = await query.ToListAsync();
            return Ok(list);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var storage = await _context.Storages.FindAsync(id);
            if(storage == null) return NotFound();
            string currentRole = GetCurrentUserRole() ?? "";
            string? unitId = GetCurrentUserUnitId();
            if(currentRole == "Manager" && unitId != null && storage.BelongToUnitId != unitId && storage.ManagerId != _context.Users.FirstOrDefault(u => u.UserName == User.Identity.Name).Id)
                return Forbid();
            if(currentRole == "User") return Forbid();
            return Ok(storage);
        }

        [HttpPost]
        public async Task<IActionResult> Create(Storage storage)
        {
            string currentRole = GetCurrentUserRole() ?? "";
            string? unitId = GetCurrentUserUnitId();
            if(currentRole == "User") return Forbid();
            if(currentRole == "Manager" && (storage.BelongToUnitId != unitId)) return Forbid();
            _context.Storages.Add(storage);
            await _context.SaveChangesAsync();
            return Ok(storage);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, Storage storage)
        {
            string currentRole = GetCurrentUserRole() ?? "";
            string? unitId = GetCurrentUserUnitId();
            if(currentRole == "User") return Forbid();
            if(currentRole == "Manager")
            {
                var origin = await _context.Storages.FindAsync(id);
                if(origin == null || (origin.BelongToUnitId != unitId && origin.ManagerId != _context.Users.FirstOrDefault(u => u.UserName == User.Identity.Name).Id))
                    return Forbid();
            }
            if(id != storage.Id) return BadRequest();
            _context.Entry(storage).State = EntityState.Modified;
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
            if(currentRole == "Manager" && (storage.BelongToUnitId != unitId && storage.ManagerId != _context.Users.FirstOrDefault(u => u.UserName == User.Identity.Name).Id))
                return Forbid();
            _context.Storages.Remove(storage);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
