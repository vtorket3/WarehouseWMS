using System.ComponentModel.DataAnnotations;

namespace WebApplication3.Models
{
    public class StockOperation
    {
        public int Id { get; set; }

        [Required]
        public string OperationType { get; set; } = "";

        public int ProductId { get; set; }

        public Product? Product { get; set; }

        public int? WarehouseId { get; set; }

        public Warehouse? Warehouse { get; set; }

        public decimal Quantity { get; set; }

        [Required]
        public string ResponsiblePerson { get; set; } = "";

        public string? Comment { get; set; }

        public DateTime CreatedAt { get; set; }
            = DateTime.Now;
    }
}