using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication3.Data;

namespace WebApplication3.Controllers
{
    public class HomeController : Controller
    {
        private readonly WarehouseDbContext _context;

        public HomeController(WarehouseDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var stock = await _context.StockBalances
                .Include(x => x.Product)
                .Include(x => x.Warehouse)
                .Include(x => x.ProductBatch)
                .ToListAsync();

            return View(stock);
        }
    }
}