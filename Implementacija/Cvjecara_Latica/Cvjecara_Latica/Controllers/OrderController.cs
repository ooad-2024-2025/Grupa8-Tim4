using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Cvjecara_Latica.Data;
using Cvjecara_Latica.Models;

namespace Cvjecara_Latica.Controllers
{
    public class OrderController : Controller
    {
        private readonly ApplicationDbContext _context;

        public OrderController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Order
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Orders.Include(o => o.Payment).Include(o => o.Person);
            return View(await applicationDbContext.ToListAsync());
        }

        public List<List<Product>> GetOrderProducts(List<Order> orders)
        {
            List<List<Product>> orderProducts = new List<List<Product>>();

            foreach (var order in orders)
            {
                var products = _context.ProductOrders
                    .Where(po => po.OrderID == order.OrderID)
                    .Select(po => po.Product)
                    .ToList();

                orderProducts.Add(products);
            }

            return orderProducts;
        }


        public IActionResult UserOrders()
        {
            var p1 = _context.Users.ToList();
            string userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            // Retrieve user-specific orders based on the PersonID
            List<Order> userOrders = _context.Orders
                .Include(o => o.Payment)
                .Where(o => o.PersonID == userId)
                .ToList();
            // Get the associated products for each order
            List<List<Product>> orderProducts = GetOrderProducts(userOrders);

            // Pass the userOrders and orderProducts to the view
            ViewBag.UserOrders = userOrders.OrderBy(order => order.Rating).ToList(); ;
            ViewBag.OrderProducts = orderProducts;

            // Render the view
            return View();
        }


        // GET: Order/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var order = await _context.Orders
                .Include(o => o.Payment)
                .Include(o => o.Person)
                .FirstOrDefaultAsync(m => m.OrderID == id);
            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }

        // GET: Order/Create
        public IActionResult Create()
        {
            ViewData["PaymentID"] = new SelectList(_context.Payments, "PaymentID", "PaymentID");
            ViewData["PersonID"] = new SelectList(_context.Set<Person>(), "Id", "Id");
            return View();
        }

        // POST: Order/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("OrderID,PaymentID,PersonID,purchaseDate,IsOrderSent,Rating,TotalAmountToPay,DeliveryDate")] Order order)
        {
            if (ModelState.IsValid)
            {
                _context.Add(order);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["PaymentID"] = new SelectList(_context.Payments, "PaymentID", "PaymentID", order.PaymentID);
            ViewData["PersonID"] = new SelectList(_context.Set<Person>(), "Id", "Id", order.PersonID);
            return View(order);
        }

        // GET: Order/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var order = await _context.Orders.FindAsync(id);
            if (order == null)
            {
                return NotFound();
            }
            ViewData["PaymentID"] = new SelectList(_context.Payments, "PaymentID", "PaymentID", order.PaymentID);
            ViewData["PersonID"] = new SelectList(_context.Set<Person>(), "Id", "Id", order.PersonID);
            return View(order);
        }

        // POST: Order/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("OrderID,PaymentID,PersonID,purchaseDate,IsOrderSent,Rating,TotalAmountToPay,DeliveryDate")] Order order)
        {
            if (id != order.OrderID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(order);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!OrderExists(order.OrderID))
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
            ViewData["PaymentID"] = new SelectList(_context.Payments, "PaymentID", "PaymentID", order.PaymentID);
            ViewData["PersonID"] = new SelectList(_context.Set<Person>(), "Id", "Id", order.PersonID);
            return View(order);
        }

        // GET: Order/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var order = await _context.Orders
                .Include(o => o.Payment)
                .Include(o => o.Person)
                .FirstOrDefaultAsync(m => m.OrderID == id);
            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }

        // POST: Order/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var order = await _context.Orders.FindAsync(id);
            if (order != null)
            {
                _context.Orders.Remove(order);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool OrderExists(int id)
        {
            return _context.Orders.Any(e => e.OrderID == id);
        }
    }
}
