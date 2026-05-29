namespace WebApplication3.Models
{
    public class Product
    {
        public int Id { get; set; }

        public string Article { get; set; } = string.Empty;

        public string Name { get; set; } = string.Empty;

        public string? Description { get; set; }

        public string Unit { get; set; } = string.Empty;

        public decimal MinStock { get; set; }
    }
}
