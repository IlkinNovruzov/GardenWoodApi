using GardenWoodAPI.Model;

namespace GardenWoodAPI.DTO
{
    public class CreateProductDTO
    {
        public string Name { get; set; } = string.Empty;
        public double Price { get; set; }
        public string? Description { get; set; }
        public int CategoryId { get; set; }
        public List<IFormFile> Images { get; set; } = [];
    }
    public class UpdateProductDTO : CreateProductDTO
    {
        public int Id { get; set; }
    }
    public class GetProductDTO
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public double Price { get; set; }
        public string? Description { get; set; }
        public int CategoryId { get; set; }
        public List<ImageDTO> Images { get; set; } = [];
    }
    public class ImageDTO
    {
        public int Id { get; set; }
        public string ImageUrl { get; set; } = string.Empty;
    }
}
