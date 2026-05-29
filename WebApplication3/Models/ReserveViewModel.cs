namespace WebApplication3.Models;

public class ReserveViewModel
{
    public int StockBalanceId { get; set; }

    public decimal Quantity { get; set; }

    public string Reason { get; set; } = "";
    public string ResponsiblePerson { get; set; } = "";
}