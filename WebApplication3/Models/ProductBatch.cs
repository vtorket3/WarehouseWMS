namespace WebApplication3.Models
{
    public class ProductBatch
    {
        public int Id { get; set; }

        public int ProductId { get; set; }

        public Product? Product { get; set; }

        public string BatchNumber { get; set; } = "";

        public DateTime? ExpirationDate { get; set; }
    }
}
