using GardenWoodAPI.Model;
using GardenWoodAPI.DTO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GardenWoodAPI.Controllers.Admin
{
    [ApiController]
    [Route("api/admin/product")]
    public class ProductController(AppDbContext context, IConfiguration config) : Controller
    {
        private readonly AppDbContext _context = context;
        private readonly IConfiguration _config = config;
        private bool IsAuthorized(string apiKey) => apiKey == _config["AdminApiKey"];

        [HttpGet("validate")]
        public IActionResult ValidateAdminKey([FromHeader(Name = "X-API-KEY")] string apiKey)
        {
            var validApiKey = _config["AdminApiKey"];

            if (apiKey != validApiKey) return Unauthorized("Geçersiz API anahtarı");

            return Ok("API key geçerli");
        }

        [HttpGet("list")]
        public async Task<IActionResult> GetAll([FromHeader(Name = "X-API-KEY")] string apiKey)
        {
            if (!IsAuthorized(apiKey)) return Unauthorized("Invalid API key");

            var products = await _context.Products.ToListAsync();
            return Ok(products);
        }

        [HttpPost("add")]
        public async Task<IActionResult> Add([FromBody] CreateProductDTO dto, [FromHeader(Name = "X-API-KEY")] string apiKey)
        {
            if (!IsAuthorized(apiKey)) return Unauthorized("Invalid API key");

            var product = new Product
            {
                Name = dto.Name,
                Price = dto.Price,
                Description = dto.Description,
                CategoryId = dto.CategoryId
            };

            _context.Products.Add(product);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Product added successfully", productId = product.Id });
        }
        [HttpPut("update/{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] CreateProductDTO dto, [FromHeader(Name = "X-API-KEY")] string apiKey)
        {
            if (!IsAuthorized(apiKey)) return Unauthorized("Invalid API key");

            var product = await _context.Products.FindAsync(id);
            if (product == null) return NotFound("Product not found");

            product.Name = dto.Name;
            product.Price = dto.Price;
            product.Description = dto.Description;
            product.CategoryId = dto.CategoryId;

            await _context.SaveChangesAsync();
            return Ok(new { message = "Product updated successfully" });
        }

        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> Delete(int id, [FromHeader(Name = "X-API-KEY")] string apiKey)
        {
            if (!IsAuthorized(apiKey)) return Unauthorized("Invalid API key");

            var product = await _context.Products.FindAsync(id);
            if (product == null) return NotFound("Product not found");

            _context.Products.Remove(product);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Product deleted successfully" });
        }
    }
}
