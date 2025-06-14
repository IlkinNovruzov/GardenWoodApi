using GardenWoodAPI.Model;
using GardenWoodAPI.DTO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GardenWoodAPI.Controllers.Admin
{
    [ApiController]
    [Route("api/admin/category")]
    public class CategoryController(AppDbContext context, IConfiguration config) : Controller
    {
        private readonly AppDbContext _context = context;
        private readonly IConfiguration _config = config;
        private bool IsAuthorized(string apiKey) => apiKey == _config["AdminApiKey"];

        //[HttpGet("list")]
        //public async Task<IActionResult> GetAll([FromHeader(Name = "X-API-KEY")] string apiKey)
        //{
        //    if (!IsAuthorized(apiKey)) return Unauthorized("Invalid API key");
        //    var categories = await _context.Categories
        //        .Select(c => new { c.Id, c.Name })
        //        .ToListAsync();
        //    return Ok(categories);
        //}
        [HttpGet("list")]
        public async Task<IActionResult> GetAll()
        {
            var categories = await _context.Categories
                .Select(c => new { c.Id, c.Name })
                .ToListAsync();
            return Ok(categories);
        }

        [HttpPost("add")]
        public async Task<IActionResult> Add([FromBody] CreateCategoryDTO categoryDto, [FromHeader(Name = "X-API-KEY")] string apiKey)
        {
            if (!IsAuthorized(apiKey)) return Unauthorized("Invalid API key");

            var category = new Category
            {
                Name = categoryDto.Name
            };
            _context.Categories.Add(category);
            await _context.SaveChangesAsync();
            return Ok(new { message = "Category added successfully." });
        }

        [HttpPut("update/{id}")]
        public async Task<IActionResult> Edit(int id, [FromBody] CreateCategoryDTO updatedDTO, [FromHeader(Name = "X-API-KEY")] string apiKey)
        {
            if (!IsAuthorized(apiKey)) return Unauthorized("Invalid API key");

            var category = await _context.Categories.FindAsync(id);
            if (category == null) return NotFound("Category not found");

            var updated = new Category
            {
                Name = updatedDTO.Name
            };
            category.Name = updated.Name;
            await _context.SaveChangesAsync();
            return Ok(new { message = "Category updated successfully." });
        }

        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> Delete(int id, [FromHeader(Name = "X-API-KEY")] string apiKey)
        {
            if (!IsAuthorized(apiKey)) return Unauthorized("Invalid API key");

            var category = await _context.Categories.FindAsync(id);
            if (category == null) return NotFound("Category not found");

            _context.Categories.Remove(category);
            await _context.SaveChangesAsync();
            return Ok(new { message = "Category deleted successfully." });
        }
    }
}
