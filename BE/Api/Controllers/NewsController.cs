using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using WebApiProject.Application.DTOs.News;
using WebApiProject.Application.IServices;

namespace WebApiProject.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class NewsController : ControllerBase
    {
        private readonly INewsService _newsService;

        public NewsController(INewsService newsService)
        {
            _newsService = newsService;
        }

        // GET: api/news - Public, không cần login
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetAll()
        {
            var news = await _newsService.GetAllAsync();
            return Ok(news);
        }

        // GET: api/news/{id} - Public, không cần login
        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> Get(int id)
        {
            var news = await _newsService.GetByIdAsync(id);
            if (news == null) return NotFound();
            return Ok(news);
        }

        // POST: api/news - Chỉ Admin và Manager
        [HttpPost]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> Create([FromBody] NewsCreateDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var userName = User.FindFirst(ClaimTypes.Name)?.Value ?? User.Identity?.Name ?? "Unknown";

            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            var news = await _newsService.CreateAsync(dto, userId, userName);
            return CreatedAtAction(nameof(Get), new { id = news.Id }, news);
        }

        // PUT: api/news/{id} - Chỉ Admin, Manager hoặc tác giả
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> Update(int id, [FromBody] NewsUpdateDto dto)
        {
            if (id != dto.Id)
            {
                return BadRequest("ID mismatch");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;
            
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            bool success;
            // Admin có thể cập nhật bất kỳ tin tức nào
            if (userRole == "Admin")
            {
                success = await _newsService.UpdateByAdminAsync(dto);
            }
            else
            {
                // Manager chỉ có thể cập nhật tin tức của mình
                success = await _newsService.UpdateAsync(dto, userId);
            }

            if (!success)
            {
                return Forbid("Bạn không có quyền cập nhật tin tức này");
            }

            return Ok(new { message = "Cập nhật thành công" });
        }

        // DELETE: api/news/{id} - Chỉ Admin, Manager hoặc tác giả
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> Delete(int id)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;
            
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            bool success;
            // Admin có thể xóa bất kỳ tin tức nào
            if (userRole == "Admin")
            {
                success = await _newsService.DeleteByAdminAsync(id);
            }
            else
            {
                // Manager chỉ có thể xóa tin tức của mình
                success = await _newsService.DeleteAsync(id, userId);
            }

            if (!success)
            {
                return Forbid("Bạn không có quyền xóa tin tức này");
            }

            return Ok(new { message = "Xóa thành công" });
        }
    }
}

