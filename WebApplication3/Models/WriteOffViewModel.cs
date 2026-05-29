namespace WebApplication3.Models;

public class WriteOffViewModel
{
    public int StockBalanceId { get; set; }

    public decimal Quantity { get; set; }
    public string ResponsiblePerson { get; set; } = "";
    public string Reason { get; set; } = string.Empty;
}