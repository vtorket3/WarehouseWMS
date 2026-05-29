namespace WebApplication3.Models;

public class TransferViewModel
{
    public int ProductId { get; set; }

    public int SourceWarehouseId { get; set; }

    public int TargetWarehouseId { get; set; }

    public decimal Quantity { get; set; }

    public string ResponsiblePerson { get; set; } = "";
}