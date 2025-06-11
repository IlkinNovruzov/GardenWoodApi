using GardenWoodAPI.DTO;
using GardenWoodAPI.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GardenWoodAPI.Controllers.Public
{
    [ApiController]
    [Route("api/product")]
    public class ProductController(AppDbContext context) : Controller
    {
        private readonly AppDbContext _context = context;

        [HttpGet("list")]
        public async Task<IActionResult> GetPaginated([FromQuery] int page = 1, [FromQuery] int pageSize = 3)
        {
            var products = await _context.Products
                .Include(p => p.Images)
                .OrderBy(p => p.Id)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(p => new GetProductDTO
                {
                    Id = p.Id,
                    Name = p.Name,
                    Price = p.Price,
                    Description = p.Description,
                    CategoryId = p.CategoryId,
                    Images = p.Images.Select(img => new ImageDTO { Id = img.Id, ImageUrl = img.ImageUrl }).ToList()
                })
                .ToListAsync();

            return Ok(products);
        }

        [HttpGet("search")]
        public async Task<IActionResult> Search(string query)
        {
            var products = await _context.Products
              .Where(p => p.Name.Contains(query))
              .Include(p => p.Images)
              .OrderBy(p => p.Id)
              //.Skip((page - 1) * pageSize)
              //.Take(pageSize)
              .Select(p => new GetProductDTO
              {
                  Id = p.Id,
                  Name = p.Name,
                  Price = p.Price,
                  Description = p.Description,
                  CategoryId = p.CategoryId,
                  Images = p.Images.Select(img => new ImageDTO { Id = img.Id, ImageUrl = img.ImageUrl }).ToList()
              })
              .ToListAsync();


            return Ok(products);
        }

    }
}
