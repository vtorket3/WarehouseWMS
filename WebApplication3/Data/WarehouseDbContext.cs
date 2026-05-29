using Microsoft.EntityFrameworkCore;
using WebApplication3.Models;
namespace WebApplication3.Data;

    public class WarehouseDbContext : DbContext
    {

        public WarehouseDbContext(

            DbContextOptions<WarehouseDbContext> options
        ) : base(options)
        {
        }

        public DbSet<Product> Products => Set<Product>();

        public DbSet<ProductBatch> ProductBatches => Set<ProductBatch>();
    public DbSet<StockOperation> StockOperations => Set<StockOperation>();
    public DbSet<StockBalance> StockBalances => Set<StockBalance>();

        public DbSet<Warehouse> Warehouses => Set<Warehouse>();

    }

