using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication3.Data;
using WebApplication3.Models;
using ClosedXML.Excel;
using System.IO;

namespace WebApplication3.Controllers
{
    public class ProductsController : Controller
    {
        private readonly WarehouseDbContext _context;

        public ProductsController(WarehouseDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var products = _context.Products
                .OrderBy(x => x.Name)
                .ToList();

            return View(products);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Product product)
        {
            if (!ModelState.IsValid)
                return View(product);

            bool exists = await _context.Products
                .AnyAsync(x => x.Article == product.Article);

            if (exists)
            {
                ModelState.AddModelError("Article", "Артикул уже существует");
                return View(product);
            }

            _context.Products.Add(product);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Товар создан";
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var product = await _context.Products.FindAsync(id);

            if (product == null)
                return NotFound();

            return View(product);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Product product)
        {
            if (!ModelState.IsValid)
                return View(product);

            var dbProduct = await _context.Products.FindAsync(product.Id);

            if (dbProduct == null)
                return NotFound();

            bool exists = await _context.Products.AnyAsync(x =>
                x.Article == product.Article &&
                x.Id != product.Id);

            if (exists)
            {
                ModelState.AddModelError("Article", "Артикул уже существует");
                return View(product);
            }

            dbProduct.Article = product.Article;
            dbProduct.Name = product.Name;
            dbProduct.Description = product.Description;

            await _context.SaveChangesAsync();

            TempData["Success"] = "Товар обновлён";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var product = await _context.Products.FindAsync(id);

            if (product == null)
                return NotFound();

            bool inStock = await _context.StockBalances
                .AnyAsync(x => x.ProductId == id);

            if (inStock)
            {
                TempData["Error"] =
                    "Нельзя удалить товар — есть остатки на складе";

                return RedirectToAction(nameof(Index));
            }

            _context.Products.Remove(product);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Товар удалён";
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var product = await _context.Products
                .FirstOrDefaultAsync(x => x.Id == id);

            if (product == null)
                return NotFound();

            return View(product);
        }

        [HttpGet]
        public IActionResult Export()
        {
            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("Products");

                worksheet.Cell(1, 1).Value = "Article";
                worksheet.Cell(1, 2).Value = "Name";
                worksheet.Cell(1, 3).Value = "Description";
                worksheet.Cell(1, 4).Value = "Unit";
                worksheet.Cell(1, 5).Value = "MinStock";

                var products = _context.Products.ToList();

                int row = 2;

                foreach (var p in products)
                {
                    worksheet.Cell(row, 1).Value = p.Article;
                    worksheet.Cell(row, 2).Value = p.Name;
                    worksheet.Cell(row, 3).Value = p.Description;
                    worksheet.Cell(row, 4).Value = p.Unit;
                    worksheet.Cell(row, 5).Value = p.MinStock;

                    row++;
                }

                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    stream.Position = 0;

                    string fileName =
                        $"products_{DateTime.Now:yyyyMMdd_HHmm}.xlsx";

                    return File(
                        stream.ToArray(),
                        "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                        fileName
                    );
                }
            }
        }

        [HttpGet]
        public IActionResult Import()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Import(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                TempData["Error"] = "Файл не выбран";
                return RedirectToAction(nameof(Index));
            }

            var newProducts = new List<Product>();

            using (var stream = file.OpenReadStream())
            using (var workbook = new XLWorkbook(stream))
            {
                var ws = workbook.Worksheet(1);
                var rows = ws.RangeUsed().RowsUsed().Skip(1);

                foreach (var row in rows)
                {
                    var article = row.Cell(1).GetString();
                    var name = row.Cell(2).GetString();

                    if (string.IsNullOrWhiteSpace(article) ||
                        string.IsNullOrWhiteSpace(name))
                        continue;

                    bool exists = await _context.Products
                        .AnyAsync(x => x.Article == article);

                    if (exists)
                        continue;

                    newProducts.Add(new Product
                    {
                        Article = article,
                        Name = name,
                        Description = row.Cell(3).GetString(),
                        Unit = row.Cell(4).GetString(),
                        MinStock = row.Cell(5).GetValue<decimal>()
                    });
                }
            }

            if (newProducts.Count > 0)
            {
                _context.Products.AddRange(newProducts);
                await _context.SaveChangesAsync();
            }

            TempData["Success"] =
                $"Импортировано товаров: {newProducts.Count}";

            return RedirectToAction(nameof(Index));
        }
    }
}