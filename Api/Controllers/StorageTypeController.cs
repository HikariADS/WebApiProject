using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApiProject.Domain.Entities;
using WebApiProject.Infrastructure.Persistence;

namespace WebApiProject.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class StorageTypeController : ControllerBase
    {
        private readonly AppDbContext _context;
        public StorageTypeController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll() => Ok(await _context.StorageTypes.ToListAsync());

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var type = await _context.StorageTypes.FindAsync(id);
            if (type == null) return NotFound();
            return Ok(type);
        }

        [HttpPost]
        public async Task<IActionResult> Create(StorageType type)
        {
            _context.StorageTypes.Add(type);
            await _context.SaveChangesAsync();
            return Ok(type);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, StorageType type)
        {
            if (id != type.Id) return BadRequest();
            _context.Entry(type).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var type = await _context.StorageTypes.FindAsync(id);
            if (type == null) return NotFound();
            _context.StorageTypes.Remove(type);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
