namespace WebApplication3.Models
{
    public class StockReceiptViewModel
    {
        public int ProductId { get; set; }

        public int WarehouseId { get; set; }

        public decimal Quantity { get; set; }

        public string BatchNumber { get; set; } = string.Empty;
        public string ResponsiblePerson { get; set; } = "";
        public DateTime? ExpirationDate { get; set; }
    }
}
