using Cvjecara_Latica.Data;
using Cvjecara_Latica.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;


namespace Cvjecara_Latica.Controllers
{
    [Authorize]
    public class OrdersController : Controller
    {
        private readonly ApplicationDbContext _context;

        public OrdersController(ApplicationDbContext context)
        {
            _context = context;
        }
      

        // GET: Orders
        /* public async Task<IActionResult> Index()
         {
             var applicationDbContext = _context.Orders.Include(o => o.Payment).Include(o => o.Person);
             return View(await applicationDbContext.ToListAsync());
         }
        */

        public async Task<IActionResult> Index()
        {
            IQueryable<Order> orders = _context.Orders
                .Include(o => o.Payment)
                .Include(o => o.Person); 

            if (!User.IsInRole("Administrator"))
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                orders = orders.Where(o => o.PersonID == userId);
            }

            return View(await orders.ToListAsync());
        }

        // GET: Orders/Details/5
        [AllowAnonymous]
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

        // GET: Orders/Create
        [Authorize(Roles = "Administrator")]
        public IActionResult Create()
        {
            ViewData["PaymentID"] = new SelectList(_context.Payments, "PaymentID", "PaymentID");
            ViewData["PersonID"] = new SelectList(_context.Set<Person>(), "Id", "Id");
            return View();
        }

        // POST: Orders/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.

        [Authorize(Roles = "Administrator")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("OrderID,PaymentID,PersonID,purchaseDate,IsOrderSent,Rating,TotalAmountToPay,DeliveryDate, IsPickedUp")] Order order)
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

        // GET: Orders/Edit/5
        [Authorize(Roles = "Administrator")]
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

        // POST: Orders/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.

        [Authorize(Roles = "Administrator")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("OrderID,PaymentID,PersonID,purchaseDate,IsOrderSent,Rating,TotalAmountToPay,DeliveryDate, IsPickedUp")] Order order)
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

        // GET: Orders/Delete/5
        [Authorize(Roles = "Administrator")]
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

        // POST: Orders/Delete/5

        [Authorize(Roles = "Administrator")]
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

        [HttpPost]
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> MarkAsPickedUp(int id)
        {
            var order = await _context.Orders.FindAsync(id);
            if (order == null)
                return NotFound();

            order.IsPickedUp = true;
            _context.Orders.Update(order);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }
    }
}
