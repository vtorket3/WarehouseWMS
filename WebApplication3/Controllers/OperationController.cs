using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication3.Data;

namespace WebApplication3.Controllers
{
    public class OperationsController : Controller
    {
        private readonly WarehouseDbContext _context;

        public OperationsController(
            WarehouseDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var operations =
                _context.StockOperations
                .Include(x => x.Product)
                .Include(x => x.Warehouse)
                .OrderByDescending(x => x.CreatedAt)
                .ToList();

            return View(operations);
        }
    }
}