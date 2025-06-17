using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Cvjecara_Latica.Data;
using Cvjecara_Latica.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;

namespace Cvjecara_Latica.Controllers
{
    [Authorize]
    public class ProductSalesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ProductSalesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: ProductSales
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.ProductSales.Include(p => p.Product);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: ProductSales/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var productSale = await _context.ProductSales
                .Include(p => p.Product)
                .FirstOrDefaultAsync(m => m.ProductSalesID == id);
            if (productSale == null)
            {
                return NotFound();
            }

            return View(productSale);
        }

        // GET: ProductSales/Create
        [Authorize(Roles="Administrator")]
        public IActionResult Create()
        {
            ViewData["ProductID"] = new SelectList(_context.Products, "ProductID", "ProductID");
            return View();
        }

        // POST: ProductSales/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ProductSalesID,SalesDate,ProductID")] ProductSale productSale)
        {
            if (ModelState.IsValid)
            {
                _context.Add(productSale);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["ProductID"] = new SelectList(_context.Products, "ProductID", "ProductID", productSale.ProductID);
            return View(productSale);
        }

        // GET: ProductSales/Edit/5
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var productSale = await _context.ProductSales.FindAsync(id);
            if (productSale == null)
            {
                return NotFound();
            }
            ViewData["ProductID"] = new SelectList(_context.Products, "ProductID", "ProductID", productSale.ProductID);
            return View(productSale);
        }

        // POST: ProductSales/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = "Administrator")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ProductSalesID,SalesDate,ProductID")] ProductSale productSale)
        {
            if (id != productSale.ProductSalesID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(productSale);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProductSaleExists(productSale.ProductSalesID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["ProductID"] = new SelectList(_context.Products, "ProductID", "ProductID", productSale.ProductID);
            return View(productSale);
        }

        // GET: ProductSales/Delete/5
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var productSale = await _context.ProductSales
                .Include(p => p.Product)
                .FirstOrDefaultAsync(m => m.ProductSalesID == id);
            if (productSale == null)
            {
                return NotFound();
            }

            return View(productSale);
        }

        // POST: ProductSales/Delete/5
        [Authorize(Roles = "Administrator")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var productSale = await _context.ProductSales.FindAsync(id);
            if (productSale != null)
            {
                _context.ProductSales.Remove(productSale);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ProductSaleExists(int id)
        {
            return _context.ProductSales.Any(e => e.ProductSalesID == id);
        }
    }
}
