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
    public class CartsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CartsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Carts
        public async Task<IActionResult> Index()
        {
          //  var applicationDbContext = _context.Cart.Include(c => c.Person).Include(c => c.Product);
          // return View(await applicationDbContext.ToListAsync());
         
        
            var userId = User.Identity.IsAuthenticated
                ? User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier).Value
                : "anonimni";

            var cartItems = _context.Cart
                .Include(c => c.Product) // samo ovo treba
                .Where(c => c.PersonID == userId) // filter po korisniku
                .ToList();

            return View(cartItems);
        }

        

        // GET: Carts/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cart = await _context.Cart
                .Include(c => c.Person)
                .Include(c => c.Product)
                .FirstOrDefaultAsync(m => m.CartID == id);
            if (cart == null)
            {
                return NotFound();
            }

            return View(cart);
        }

        // GET: Carts/Create
        public IActionResult Create()
        {
            ViewData["PersonID"] = new SelectList(_context.Set<Person>(), "Id", "Id");
            ViewData["ProductID"] = new SelectList(_context.Products, "ProductID", "ProductID");
            return View();
        }

        // POST: Carts/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("CartID,ProductID,PersonID,ProductQuantity")] Cart cart)
        {
            if (ModelState.IsValid)
            {
                _context.Add(cart);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["PersonID"] = new SelectList(_context.Set<Person>(), "Id", "Id", cart.PersonID);
            ViewData["ProductID"] = new SelectList(_context.Products, "ProductID", "ProductID", cart.ProductID);
            return View(cart);
        }

        // GET: Carts/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cart = await _context.Cart.FindAsync(id);
            if (cart == null)
            {
                return NotFound();
            }
            ViewData["PersonID"] = new SelectList(_context.Set<Person>(), "Id", "Id", cart.PersonID);
            ViewData["ProductID"] = new SelectList(_context.Products, "ProductID", "ProductID", cart.ProductID);
            return View(cart);
        }

        // POST: Carts/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("CartID,ProductID,PersonID,ProductQuantity")] Cart cart)
        {
            if (id != cart.CartID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(cart);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CartExists(cart.CartID))
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
            ViewData["PersonID"] = new SelectList(_context.Set<Person>(), "Id", "Id", cart.PersonID);
            ViewData["ProductID"] = new SelectList(_context.Products, "ProductID", "ProductID", cart.ProductID);
            return View(cart);
        }

        // GET: Carts/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cart = await _context.Cart
                .Include(c => c.Person)
                .Include(c => c.Product)
                .FirstOrDefaultAsync(m => m.CartID == id);
            if (cart == null)
            {
                return NotFound();
            }

            return View(cart);
        }

        // POST: Carts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [HttpPost]
       
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var cartItem = await _context.Cart.FindAsync(id);
            if (cartItem != null)
            {
                _context.Cart.Remove(cartItem);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        private bool CartExists(int id)
        {
            return _context.Cart.Any(e => e.CartID == id);
        }
        [HttpPost]
        public IActionResult AddToCart(int productId)
        {
            // Provjeri da li proizvod postoji
            var product = _context.Products.FirstOrDefault(p => p.ProductID == productId);
            if (product == null)
                return NotFound();

            // Ako koristiš autentifikaciju — dohvaća se ID prijavljenog korisnika
            var userId = User.Identity.IsAuthenticated
                ? User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier).Value
                : "anonimni"; // zamijeni ako ne koristiš korisnike

            // Provjeri postoji li već isti proizvod u korpi korisnika
            var existingCartItem = _context.Cart
                .FirstOrDefault(c => c.ProductID == productId && c.PersonID == userId);

            if (existingCartItem != null)
            {
                existingCartItem.ProductQuantity = (existingCartItem.ProductQuantity ?? 1) + 1;
            }
            else
            {
                var cartItem = new Cart
                {
                    ProductID = productId,
                    PersonID = userId,
                    ProductQuantity = 1
                };
                _context.Cart.Add(cartItem);
            }

            _context.SaveChanges();

            // Vrati korisnika nazad na stranicu proizvoda
            return RedirectToAction("Details", "Products", new { id = productId });
        }
        [HttpGet]
        public IActionResult PlaceOrder()
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            var cartItems = _context.Cart
                .Include(c => c.Product)
                .Where(c => c.PersonID == userId)
                .ToList();

            double total = cartItems.Sum(item => item.Product.Price * (item.ProductQuantity ?? 1));
            ViewBag.Total = total;

            // Spremi kodove popusta korisnika (ako je registriran)
            var discounts = _context.Discounts
                .Where(d => d.PersonID == userId && !d.IsUsed && d.DiscountBegins <= DateTime.Now)
                .ToList();

            ViewBag.Discounts = discounts;

            return View();
        }

        // POST: Carts/SubmitOrder
        [HttpPost]
        public async Task<IActionResult> SubmitOrder(string name, string discount, string city, string address, string phone, string applyDiscount)
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            var cartItems = _context.Cart.Include(c => c.Product).Where(c => c.PersonID == userId).ToList();

            double total = cartItems.Sum(item => item.Product.Price * (item.ProductQuantity ?? 1));
            double originalTotal = total;
            double discountedTotal = total;

            // Ako je kliknuto "Apply" za popust kod
            if (!string.IsNullOrEmpty(applyDiscount))
            {
                if (!string.IsNullOrEmpty(discount))
                {
                    var validDiscount = _context.Discounts
                        .FirstOrDefault(d => d.DiscountCode == discount && d.PersonID == userId && !d.IsUsed && d.DiscountBegins <= DateTime.Now);

                    if (validDiscount != null)
                    {
                        if (validDiscount.DiscountType == DiscountType.PercentageOff)
                            discountedTotal = total - (total * validDiscount.DiscountAmount / 100);
                        else if (validDiscount.DiscountType == DiscountType.AmountOff)
                            discountedTotal = total - validDiscount.DiscountAmount;

                        // Ažuriraj popust kod u ViewBag za prikaz na stranici
                        ViewBag.AppliedCode = discount;
                        ViewBag.DiscountedTotal = discountedTotal;

                        // Označi popust kao iskorišten
                        validDiscount.IsUsed = true;
                        _context.Discounts.Update(validDiscount);
                        await _context.SaveChangesAsync();
                    }
                    else
                    {
                        ViewBag.Error = "Invalid or expired discount code.";
                    }
                }
            }

            // Pošaljite korisniku na potvrdu narudžbe s novim informacijama
            ViewBag.Total = originalTotal;
            ViewBag.Name = name;
            ViewBag.City = city;
            ViewBag.Address = address;
            ViewBag.Phone = phone;

            // Ispisujemo ukupnu cijenu (s popustom ili bez njega)
            ViewBag.DiscountedTotal = discountedTotal;

            // Prikazivanje stranice za potvrdu narudžbe
            return View("ConfirmOrder");
        }

        // GET: Carts/ConfirmOrder
        public IActionResult ConfirmOrder(string name, string discount, string city, string address, string phone)
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

            var cartItems = _context.Cart
                .Include(c => c.Product)
                .Where(c => c.PersonID == userId)
                .ToList();

            double total = cartItems.Sum(item => item.Product.Price * (item.ProductQuantity ?? 1));
            double discountedTotal = total;

            // Ako je popust kod uneseno na stranici za potvrdu
            if (!string.IsNullOrEmpty(discount))
            {
                var discountEntity = _context.Discounts
                    .FirstOrDefault(d =>
                        d.DiscountCode == discount &&
                        d.DiscountBegins <= DateTime.Now &&
                        !d.IsUsed &&
                        d.PersonID == userId);

                if (discountEntity != null && discountEntity.DiscountAmount > 0)
                {
                    // Primijeni popust
                    discountedTotal = discountEntity.DiscountType == DiscountType.PercentageOff
                        ? total - (total * discountEntity.DiscountAmount / 100)
                        : total - discountEntity.DiscountAmount;

                    // Označi popust kao iskorišten
                    discountEntity.IsUsed = true;
                    _context.SaveChanges();

                    // Postavi info za prikaz
                    ViewBag.AppliedCode = discount;
                    ViewBag.Total = total;
                    ViewBag.DiscountedTotal = discountedTotal;
                    ViewBag.Name = name;
                    ViewBag.City = city;
                    ViewBag.Address = address;
                    ViewBag.Phone = phone;

                    return View("ConfirmOrder");
                }
                else
                {
                    ViewBag.Error = "The discount code is invalid or already used.";
                    ViewBag.Total = total;
                    ViewBag.Name = name;
                    ViewBag.City = city;
                    ViewBag.Address = address;
                    ViewBag.Phone = phone;

                    return View("PlaceOrder");
                }
            }

            // Ako nema koda uopšte
            ViewBag.Total = total;
            ViewBag.DiscountedTotal = discountedTotal;
            ViewBag.Name = name;
            ViewBag.City = city;
            ViewBag.Address = address;
            ViewBag.Phone = phone;

            return View("ConfirmOrder");
        }



    }

}
