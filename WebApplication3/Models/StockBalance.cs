using System.ComponentModel.DataAnnotations.Schema;

namespace WebApplication3.Models
{
    public class StockBalance
    {
        public int Id { get; set; }

        public int WarehouseId { get; set; }

        public int ProductId { get; set; }

        public int? BatchId { get; set; }

        [ForeignKey(nameof(BatchId))]
        public ProductBatch? ProductBatch { get; set; }

        public decimal Quantity { get; set; }

        public decimal ReservedQuantity { get; set; }

        public Warehouse? Warehouse { get; set; }
        public Product? Product { get; set; }

    }
}
