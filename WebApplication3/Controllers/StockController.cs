using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication3.Data;
using WebApplication3.Models;

namespace WebApplication3.Controllers
{
    public class StockController : Controller
    {
        private readonly WarehouseDbContext _context;

        public StockController(WarehouseDbContext context)
        {
            _context = context;
        }

        public IActionResult Report(DateTime? from, DateTime? to)
        {
            var query = _context.StockOperations
                .Include(x => x.Product)
                .Include(x => x.Warehouse)
                .AsQueryable();

            if (from.HasValue)
                query = query.Where(x => x.CreatedAt >= from);

            if (to.HasValue)
                query = query.Where(x => x.CreatedAt <= to);

            var report = query
                .GroupBy(x => new
                {
                    x.ProductId,
                    x.Product.Name
                })
                .Select(g => new
                {
                    Product = g.Key.Name,
                    Income = g.Where(x => x.OperationType == "ПРИЕМКА").Sum(x => x.Quantity),
                    Expense = g.Where(x => x.OperationType == "СПИСАНИЕ").Sum(x => x.Quantity),
                    Transfer = g.Where(x => x.OperationType == "ПЕРЕМЕЩЕНИЕ").Sum(x => x.Quantity)
                })
                .ToList();

            return View(report);
        }
        public async Task<IActionResult> Index()
        {
            var stock = await _context.StockBalances
                .Include(x => x.Product)
                .Include(x => x.ProductBatch)
                .Include(x => x.Warehouse)
                .ToListAsync();

            var today = DateTime.Today;

            foreach (var item in stock)
            {
                var batch = item.ProductBatch;

                if (batch?.ExpirationDate != null &&
                    batch.ExpirationDate.Value.Date < today)
                {
                    if (item.Quantity > 0)
                    {
                        item.Quantity = 0;
                        item.ReservedQuantity = 0;

                        _context.StockOperations.Add(
                            new StockOperation
                            {
                                OperationType = "СПИСАНИЕ (ПРОСРОЧКА)",
                                ProductId = item.ProductId,
                                WarehouseId = item.WarehouseId,
                                Quantity = item.Quantity,
                                ResponsiblePerson = "SYSTEM",
                                Comment = $"Автоматическое списание партии {batch.BatchNumber}"
                            });
                    }
                }
            }

            await _context.SaveChangesAsync();

            return View(stock);
        }

        [HttpGet]
        public IActionResult Receipt()
        {
            ViewBag.Products = _context.Products.ToList();
            ViewBag.Warehouses = _context.Warehouses.ToList();
            return View(new StockReceiptViewModel());
        }

        [HttpPost]
        public async Task<IActionResult> Receipt(StockReceiptViewModel model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Products = _context.Products.ToList();
                ViewBag.Warehouses = _context.Warehouses.ToList();
                return View(model);
            }

            var batch = new ProductBatch
            {
                ProductId = model.ProductId,
                BatchNumber = model.BatchNumber,
                ExpirationDate = model.ExpirationDate
            };

            _context.ProductBatches.Add(batch);
            await _context.SaveChangesAsync();

            var stock = new StockBalance
            {
                ProductId = model.ProductId,
                WarehouseId = model.WarehouseId,
                BatchId = batch.Id,
                Quantity = model.Quantity,
                ReservedQuantity = 0
            };

            _context.StockBalances.Add(stock);

            _context.StockOperations.Add(
                new StockOperation
                {
                    OperationType = "ПРИЕМКА",
                    ProductId = model.ProductId,
                    WarehouseId = model.WarehouseId,
                    Quantity = model.Quantity,
                    ResponsiblePerson = model.ResponsiblePerson,
                    Comment = $"Партия: {model.BatchNumber}"
                });

            await _context.SaveChangesAsync();

            TempData["Success"] = "Поступление успешно выполнено";

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var stock = await _context.StockBalances.FindAsync(id);

            if (stock == null)
                return NotFound();

            if (stock.ReservedQuantity > 0)
            {
                TempData["Error"] = "Нельзя удалить остаток с резервом";
                return RedirectToAction(nameof(Index));
            }

            _context.StockBalances.Remove(stock);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Запись склада удалена";

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public IActionResult Transfer()
        {
            ViewBag.Products = _context.Products.ToList();
            ViewBag.Warehouses = _context.Warehouses.ToList();
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Transfer(TransferViewModel model)
        {
            if (model.SourceWarehouseId == model.TargetWarehouseId)
            {
                TempData["Error"] = "Склады совпадают";
                return RedirectToAction(nameof(Transfer));
            }

            var sourceStock = await _context.StockBalances
                .FirstOrDefaultAsync(x =>
                    x.ProductId == model.ProductId &&
                    x.WarehouseId == model.SourceWarehouseId);

            if (sourceStock == null || sourceStock.Quantity < model.Quantity)
            {
                TempData["Error"] = "Недостаточно товара";
                return RedirectToAction(nameof(Transfer));
            }

            sourceStock.Quantity -= model.Quantity;

            var targetStock = await _context.StockBalances
                .FirstOrDefaultAsync(x =>
                    x.ProductId == model.ProductId &&
                    x.WarehouseId == model.TargetWarehouseId);

            if (targetStock == null)
            {
                targetStock = new StockBalance
                {
                    ProductId = model.ProductId,
                    WarehouseId = model.TargetWarehouseId,
                    Quantity = 0,
                    ReservedQuantity = 0
                };

                _context.StockBalances.Add(targetStock);
            }

            targetStock.Quantity += model.Quantity;

            _context.StockOperations.Add(
                new StockOperation
                {
                    OperationType = "ПЕРЕМЕЩЕНИЕ",
                    ProductId = model.ProductId,
                    WarehouseId = model.SourceWarehouseId,
                    Quantity = model.Quantity,
                    ResponsiblePerson = model.ResponsiblePerson,
                    Comment = $"из {model.SourceWarehouseId} в {model.TargetWarehouseId}"
                });

            await _context.SaveChangesAsync();

            TempData["Success"] = "Перемещение выполнено";

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Reserve(int id)
        {
            var stock = await _context.StockBalances
                .Include(x => x.Product)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (stock == null)
                return NotFound();

            ViewBag.Stock = stock;

            return View(new ReserveViewModel
            {
                StockBalanceId = stock.Id
            });
        }

        [HttpPost]
        public async Task<IActionResult> Reserve(ReserveViewModel model)
        {
            var stock = await _context.StockBalances
                .FirstOrDefaultAsync(x => x.Id == model.StockBalanceId);

            if (stock == null)
                return NotFound();

            var available = stock.Quantity - stock.ReservedQuantity;

            if (model.Quantity > available)
            {
                TempData["Error"] = "Недостаточно товара";
                return RedirectToAction(nameof(Reserve), new { id = stock.Id });
            }

            stock.ReservedQuantity += model.Quantity;

            _context.StockOperations.Add(
                new StockOperation
                {
                    OperationType = "РЕЗЕРВ",
                    ProductId = stock.ProductId,
                    WarehouseId = stock.WarehouseId,
                    Quantity = model.Quantity,
                    ResponsiblePerson = model.ResponsiblePerson
                });

            await _context.SaveChangesAsync();

            TempData["Success"] = "Резерв создан";

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> WriteOff(int id)
        {
            var stock = await _context.StockBalances
                .Include(x => x.Product)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (stock == null)
                return NotFound();

            ViewBag.Stock = stock;

            return View(new WriteOffViewModel
            {
                StockBalanceId = stock.Id
            });
        }

        [HttpPost]
        public async Task<IActionResult> WriteOff(WriteOffViewModel model)
        {
            var stock = await _context.StockBalances
                .FirstOrDefaultAsync(x => x.Id == model.StockBalanceId);

            if (stock == null)
                return NotFound();

            if (model.Quantity <= 0)
            {
                TempData["Error"] = "Некорректное количество";
                return RedirectToAction(nameof(Index));
            }

            if (model.Quantity > stock.Quantity)
            {
                TempData["Error"] = "Недостаточно товара";
                return RedirectToAction(nameof(Index));
            }

            stock.Quantity -= model.Quantity;

            if (stock.Quantity < 0)
                stock.Quantity = 0;

            _context.StockOperations.Add(
                new StockOperation
                {
                    OperationType = "СПИСАНИЕ",
                    ProductId = stock.ProductId,
                    WarehouseId = stock.WarehouseId,
                    Quantity = model.Quantity,
                    ResponsiblePerson = model.ResponsiblePerson
                });

            await _context.SaveChangesAsync();

            TempData["Success"] = "Списано";

            return RedirectToAction(nameof(Index));
        }
    }
}