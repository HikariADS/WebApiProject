using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApiProject.Application.DTOs.Paging;
using WebApiProject.Application.DTOs.Product;
using WebApiProject.Application.IServices;
using WebApiProject.Domain.Entities;
using WebApiProject.Infrastructure.Persistence;

namespace WebApiProject.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductController : ControllerBase
    {
        private readonly IProductService _service;
        public ProductController(IProductService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] PageRequest request)
        {
            var result = await _service.GetAllAsync(request);
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var product = await _service.GetByIdAsync(id);
            if (product == null) return NotFound();
            return Ok(product);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] ProductCreateDto dto)
        {
           var newItem = await _service.CreateAsync(dto);
           return Ok(newItem);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update([FromBody] ProductUpdateDto dto)
        {
            var success = await _service.UpdateAsync(dto);
            return Ok(success);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var success = await _service.DeleteAsync(id);
            return Ok(success);
        }
    }
}
