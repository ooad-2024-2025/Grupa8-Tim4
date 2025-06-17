using Cvjecara_Latica.Data;
using Cvjecara_Latica.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cvjecara_Latica.Controllers
{
    [Authorize]

    public class ProductOrdersController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ProductOrdersController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: ProductOrders
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.ProductOrders.Include(p => p.Order).Include(p => p.Product);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: ProductOrders/Details/5
        [AllowAnonymous]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var productOrder = await _context.ProductOrders
                .Include(p => p.Order)
                .Include(p => p.Product)
                .FirstOrDefaultAsync(m => m.ProductOrderID == id);
            if (productOrder == null)
            {
                return NotFound();
            }

            return View(productOrder);
        }

        // GET: ProductOrders/Create
        [Authorize(Roles = "Administrator")]
        public IActionResult Create()
        {
            ViewData["OrderID"] = new SelectList(_context.Orders, "OrderID", "OrderID");
            ViewData["ProductID"] = new SelectList(_context.Products, "ProductID", "ProductID");
            return View();
        }

        // POST: ProductOrders/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.

        [Authorize(Roles = "Administrator")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ProductOrderID,OrderID,ProductID,ProductQuantity")] ProductOrder productOrder)
        {
            if (ModelState.IsValid)
            {
                _context.Add(productOrder);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["OrderID"] = new SelectList(_context.Orders, "OrderID", "OrderID", productOrder.OrderID);
            ViewData["ProductID"] = new SelectList(_context.Products, "ProductID", "ProductID", productOrder.ProductID);
            return View(productOrder);
        }

        // GET: ProductOrders/Edit/5
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var productOrder = await _context.ProductOrders.FindAsync(id);
            if (productOrder == null)
            {
                return NotFound();
            }
            ViewData["OrderID"] = new SelectList(_context.Orders, "OrderID", "OrderID", productOrder.OrderID);
            ViewData["ProductID"] = new SelectList(_context.Products, "ProductID", "ProductID", productOrder.ProductID);
            return View(productOrder);
        }

        // POST: ProductOrders/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = "Administrator")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ProductOrderID,OrderID,ProductID,ProductQuantity")] ProductOrder productOrder)
        {
            if (id != productOrder.ProductOrderID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(productOrder);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProductOrderExists(productOrder.ProductOrderID))
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
            ViewData["OrderID"] = new SelectList(_context.Orders, "OrderID", "OrderID", productOrder.OrderID);
            ViewData["ProductID"] = new SelectList(_context.Products, "ProductID", "ProductID", productOrder.ProductID);
            return View(productOrder);
        }

        // GET: ProductOrders/Delete/5
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var productOrder = await _context.ProductOrders
                .Include(p => p.Order)
                .Include(p => p.Product)
                .FirstOrDefaultAsync(m => m.ProductOrderID == id);
            if (productOrder == null)
            {
                return NotFound();
            }

            return View(productOrder);
        }

        // POST: ProductOrders/Delete/5
        [Authorize(Roles = "Administrator")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var productOrder = await _context.ProductOrders.FindAsync(id);
            if (productOrder != null)
            {
                _context.ProductOrders.Remove(productOrder);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ProductOrderExists(int id)
        {
            return _context.ProductOrders.Any(e => e.ProductOrderID == id);
        }
    }
}
