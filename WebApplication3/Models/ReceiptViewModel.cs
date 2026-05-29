namespace WebApplication3.Models
{
    public class ReceiptViewModel
    {
        public int ProductId { get; set; }

        public decimal Quantity { get; set; }

        public string BatchNumber { get; set; } = "";

        public DateTime? ExpirationDate { get; set; }
    }
}
