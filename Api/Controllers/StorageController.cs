using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApiProject.Domain.Entities;
using WebApiProject.Infrastructure.Persistence;

namespace WebApiProject.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class StorageController : ControllerBase
    {
        private readonly AppDbContext _context;
        public StorageController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll() => Ok(await _context.Storages.ToListAsync());

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var storage = await _context.Storages.FindAsync(id);
            if (storage == null) return NotFound();
            return Ok(storage);
        }

        [HttpPost]
        public async Task<IActionResult> Create(Storage storage)
        {
            _context.Storages.Add(storage);
            await _context.SaveChangesAsync();
            return Ok(storage);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, Storage storage)
        {
            if (id != storage.Id) return BadRequest();
            _context.Entry(storage).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var storage = await _context.Storages.FindAsync(id);
            if (storage == null) return NotFound();
            _context.Storages.Remove(storage);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
