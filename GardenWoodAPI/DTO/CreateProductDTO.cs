namespace GardenWoodAPI.DTO
{
    public class CreateProductDTO
    {
        public string Name { get; set; } = string.Empty;
        public double Price { get; set; }
        public string? Description { get; set; }
        public int CategoryId { get; set; }
    }
    public class UpdateProductDTO : CreateProductDTO
    {
        public int Id { get; set; }
    }
}
