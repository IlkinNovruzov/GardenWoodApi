using GardenWoodAPI.Model;
using GardenWoodAPI.DTO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GardenWoodAPI.Services;

namespace GardenWoodAPI.Controllers.Admin
{
    [ApiController]
    [Route("api/admin/product")]
    public class ProductController(AppDbContext context, IConfiguration config, FirebaseStorage firebaseStorage) : Controller
    {
        private readonly AppDbContext _context = context;
        private readonly IConfiguration _config = config;
        private readonly FirebaseStorage _firebaseStorage = firebaseStorage;

        private bool IsAuthorized(string apiKey) => apiKey == _config["AdminApiKey"];


        [HttpGet("list")]
        public async Task<IActionResult> GetAll([FromHeader(Name = "X-API-KEY")] string apiKey)
        {
            if (!IsAuthorized(apiKey)) return Unauthorized("Invalid API key");

            var productDTO = await _context.Products
                .Include(p => p.Images)
                .Select(p => new GetProductDTO
                {
                    Id = p.Id,
                    Name = p.Name,
                    Price = p.Price,
                    Description = p.Description,
                    CategoryId = p.CategoryId,
                    Images = p.Images.Select(img => new ImageDTO
                    {
                        Id = img.Id,
                        ImageUrl = img.ImageUrl
                    }).ToList()
                })
                .ToListAsync();

            return Ok(productDTO);
        }


        [HttpPost("add")]
        public async Task<IActionResult> Add([FromForm] CreateProductDTO dto, [FromHeader(Name = "X-API-KEY")] string apiKey)
        {
            if (!IsAuthorized(apiKey)) return Unauthorized("Invalid API key");
            if (ModelState.IsValid == false) return BadRequest(ModelState);
            var product = new Product
            {
                Name = dto.Name,
                Price = dto.Price,
                Description = dto.Description,
                CategoryId = dto.CategoryId,
                Images = []
            };

            foreach (var image in dto.Images)
            {
                if (image.Length > 0)
                {
                    var imageUrl = await _firebaseStorage.UploadFileAsync(image);
                    product.Images.Add(new ProductImage
                    {
                        ImageUrl = imageUrl
                    });
                }
            }

            _context.Products.Add(product);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Product added successfully", productId = product.Id });
        }

        [HttpPut("update/{id}")]
        public async Task<IActionResult> Update(int id, [FromForm] UpdateProductDTO dto, [FromHeader(Name = "X-API-KEY")] string apiKey)
        {
            if (!IsAuthorized(apiKey)) return Unauthorized("Invalid API key");

            var product = await _context.Products.Include(p => p.Images).FirstOrDefaultAsync(p => p.Id == id);

            if (product == null) return NotFound("Product not found");

            product.Name = dto.Name;
            product.Price = dto.Price;
            product.Description = dto.Description;
            product.CategoryId = dto.CategoryId;

            if (dto.Images != null && dto.Images.Count > 0)
            {
                // Önce eski resimleri sil (istersen burada dosya sisteminden/firabase'den de silme işlemi yap)
               // _context.ProductImage.RemoveRange(product.Images);
                foreach (var file in dto.Images)
                {
                    var imageUrl = await _firebaseStorage.UploadFileAsync(file);
                    product.Images?.Add(new ProductImage
                    {
                        ImageUrl = imageUrl,
                        ProductId = product.Id
                    });
                }
            }

            await _context.SaveChangesAsync();

            return Ok(new { message = "Product updated successfully" });
        }

        [HttpDelete("delete/image/{imageId}")]
        public async Task<IActionResult> DeleteProductImage(int imageId, [FromHeader(Name = "X-API-KEY")] string apiKey)
        {
            if (!IsAuthorized(apiKey)) return Unauthorized("Invalid API key");

            var image = await _context.ProductImages.FindAsync(imageId);
            if (image == null) return NotFound("Image not found");

            // Eğer Firebase ya da başka yerde dosya varsa oradan da sil
            //  await _firebaseStorage.DeleteFileAsync(image.ImageUrl);

            _context.ProductImages.Remove(image);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Image deleted successfully" });
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

        [HttpGet("validate")]
        public IActionResult ValidateAdminKey([FromHeader(Name = "X-API-KEY")] string apiKey)
        {
            var validApiKey = _config["AdminApiKey"];

            if (apiKey != validApiKey) return Unauthorized("Geçersiz API anahtarı");

            return Ok("API key geçerli");
        }
    }
}
