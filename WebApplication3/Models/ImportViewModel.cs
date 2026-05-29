namespace WebApplication3.Models
{
    public class ProductImportRow
    {
        public string Article { get; set; } = "";
        public string Name { get; set; } = "";
        public string Description { get; set; } = "";
        public string Unit { get; set; } = "";
        public decimal MinStock { get; set; }
    }
}