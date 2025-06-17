using Cvjecara_Latica.Data;
using Cvjecara_Latica.Models;
using Cvjecara_Latica.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;


namespace Cvjecara_Latica.Controllers
{
    [Authorize]
    public class CartsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly EmailService _emailService;

        public CartsController(ApplicationDbContext context, EmailService emailService)
        {
            _context = context;
            _emailService = emailService;
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
                return NotFound();

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var cart = await _context.Cart
                .Include(c => c.Product)
                .FirstOrDefaultAsync(c => c.CartID == id &&( c.PersonID == userId || User.IsInRole("Administrator")));

            if (cart == null)
                return Forbid(); // ili NotFound()

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
                return NotFound();

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var cart = await _context.Cart
                .Include(c => c.Product)
                .FirstOrDefaultAsync(c => c.CartID == id && (c.PersonID == userId || User.IsInRole("Administrator")));

            if (cart == null)
                return Forbid(); // ili NotFound()

            ViewData["ProductID"] = new SelectList(_context.Products, "ProductID", "ProductID", cart.ProductID);
            ViewData["PersonID"] = new SelectList(_context.Users, "Id", "Id", cart.PersonID);
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
                return NotFound();

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var originalCart = await _context.Cart
                .FirstOrDefaultAsync(c => c.CartID == id && (c.PersonID == userId) || User.IsInRole("Administrator"));

            if (originalCart == null)
                return Forbid();

            if (ModelState.IsValid)
            {
                originalCart.ProductQuantity = cart.ProductQuantity;
                _context.Update(originalCart);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewData["ProductID"] = new SelectList(_context.Products, "ProductID", "ProductID", cart.ProductID);
            ViewData["PersonID"] = new SelectList(_context.Users, "Id", "Id", cart.PersonID);
            return View(cart);
        }

        // GET: Carts/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return NotFound();

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var cart = await _context.Cart
                .Include(c => c.Product)
                .FirstOrDefaultAsync(c => c.CartID == id &&( c.PersonID == userId || User.IsInRole("Administrator")));

            if (cart == null)
                return Forbid();

            return View(cart);
        }
        
        // POST: Carts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var cartItem = await _context.Cart
                .FirstOrDefaultAsync(c => c.CartID == id && (c.PersonID == userId || User.IsInRole("Administrator")));

            if (cartItem == null)
                return Forbid();

            _context.Cart.Remove(cartItem);
            await _context.SaveChangesAsync();

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
                .FirstOrDefault(c => c.ProductID == productId && (c.PersonID == userId || User.IsInRole("Administrator")));

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

            return View();
        }

        // POST: Carts/SubmitOrder
        [HttpPost]
        
        public async Task<IActionResult> SubmitOrder(string name, string city, string address, string phone, DateTime deliveryDate, string discountCode)
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            var cartItems = _context.Cart
                .Include(c => c.Product)
                .Where(c => c.PersonID == userId)
                .ToList();

            double total = cartItems.Sum(item => item.Product.Price * (item.ProductQuantity ?? 1));
            double originalTotal = total;

            // Validacija datuma isporuke
            if (deliveryDate < DateTime.Today.AddDays(2) ||
                deliveryDate.Hour < 9 ||
                deliveryDate.Hour >= 17)
            {
                ModelState.AddModelError("deliveryDate", "Delivery must be at least 2 days from today, between 09:00 and 17:00.");
                ViewBag.Total = total;
                return View("PlaceOrder");
            }

            // Obrada promo koda
            if (!string.IsNullOrWhiteSpace(discountCode))
            {
                var discount = _context.Discounts.FirstOrDefault(d =>
                    d.DiscountCode == discountCode &&
                    !d.IsUsed &&
                    //d.DiscountBegins <= DateTime.Now &&
                     d.PersonID == userId);

                if (discount != null)
                {
                    if (discount.DiscountType == DiscountType.PercentageOff)
                    {
                        total -= total * discount.DiscountAmount / 100;
                    }
                    else // AmountOff
                    {
                        total -= discount.DiscountAmount;
                    }

                    if (total < 0)
                        total = 0;

                    // Oznaka da je iskorišten
                    discount.IsUsed = true;
                    _context.SaveChanges();
                    TempData["DiscountApplied"] = true;
                    TempData["OriginalTotal"] = originalTotal;
                    TempData["SavedAmount"] = originalTotal - total;

                    TempData["DiscountMessage"] = $"Promo code successfully applied! You saved\" {(originalTotal - total):F2} USD.";
                }
                else
                {
                    TempData["DiscountApplied"] = false;
                    TempData["DiscountMessage"] = "The entered promo code is not valid or has already been used.";
                }
            }

            // Slanje podataka na stranicu za potvrdu narudžbe
            ViewBag.Total = total;
            ViewBag.Name = name;
            ViewBag.City = city;
            ViewBag.Address = address;
            ViewBag.Phone = phone;
            ViewBag.DeliveryDate = deliveryDate;
            ViewBag.DiscountCode = discountCode;

            return View("ConfirmOrder");
        }

        // GET: Carts/ConfirmOrder
        // GET: Carts/ConfirmOrder
        public IActionResult ConfirmOrder(string name, string city, string address, string phone)
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

            var cartItems = _context.Cart
                .Include(c => c.Product)
                .Where(c => c.PersonID == userId)
                .ToList();

            double total = cartItems.Sum(item => item.Product.Price * (item.ProductQuantity ?? 1));

            ViewBag.Total = total;
            ViewBag.Name = name;
            ViewBag.City = city;
            ViewBag.Address = address;
            ViewBag.Phone = phone;

            return View("ConfirmOrder");
        }

        [HttpPost]
        public async Task<IActionResult> SubmitOrderFinal(
     string name,
     string city,
     string address,
     string phone,
     string paymentTypeString,
     double total,
     string? BankAccount,
     DateTime deliveryDate)
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

            var cartItems = _context.Cart
                .Include(c => c.Product)
                .Where(c => c.PersonID == userId)
                .ToList();

            if (!cartItems.Any())
            {
                ViewBag.Error = "Your cart is empty.";
                return RedirectToAction("Index");
            }

            if (paymentTypeString == "Card" && string.IsNullOrWhiteSpace(BankAccount))
            {
                ModelState.AddModelError("BankAccount", "Card number is required when paying by card.");
            }

            if (!ModelState.IsValid)
            {
                ViewBag.Name = name;
                ViewBag.City = city;
                ViewBag.Address = address;
                ViewBag.Phone = phone;
                ViewBag.DeliveryDate = deliveryDate;
                ViewBag.Total = total;
                return View("ConfirmOrder", new Payment());
            }

            // Konverzija stringa u enum
            var selectedPaymentType = paymentTypeString == "Delivery"
                ? Cvjecara_Latica.Models.PaymentType.Cash
                : Cvjecara_Latica.Models.PaymentType.CreditCard;

            var payment = new Payment
            {
                DeliveryAddress = address,
                PayedAmount = total,
                PaymentType = selectedPaymentType,
                BankAccount = BankAccount?.ToString()
            };

            _context.Payments.Add(payment);
            await _context.SaveChangesAsync();


            var order = new Order
            {
                PersonID = userId,
                purchaseDate = DateTime.Now,
                TotalAmountToPay = total,
                IsOrderSent = false,
                DeliveryDate = deliveryDate,
                PaymentID = payment.PaymentID
            };

            _context.Orders.Add(order);

            var user = await _context.Users.FindAsync(userId);
            if (user != null)
            {
                var productDetails = string.Join("<br/>", cartItems.Select(ci =>
                    $"- {ci.Product.Name} x {ci.ProductQuantity} = {ci.Product.Price * ci.ProductQuantity} USD"));

                var emailBody = $@"
            <h3>Order Confirmation</h3>
            <p><strong>Name:</strong> {name}</p>
            <p><strong>City:</strong> {city}</p>
            <p><strong>Address:</strong> {address}</p>
            <p><strong>Phone:</strong> {phone}</p>
            <p><strong>Delivery Date:</strong>{deliveryDate}</p>
            <p><strong>Payment Method:</strong> {(selectedPaymentType == PaymentType.CreditCard ? "Credit Card" : "Cash on Delivery")}</p>
            <p><strong>Total Amount:</strong> {total} USD</p>
            <p><strong>Products:</strong><br/>{productDetails}</p>";

                await _emailService.SendEmailAsync(user.Email, "Your order was successful", emailBody);
            }

            _context.Cart.RemoveRange(cartItems);
            await _context.SaveChangesAsync();

            foreach (var item in cartItems)
            {
                var productOrder = new ProductOrder
                {
                    OrderID = order.OrderID,
                    ProductID = item.ProductID,
                    ProductQuantity = item.ProductQuantity ?? 1
                };
                _context.ProductOrders.Add(productOrder);
            }
            await _context.SaveChangesAsync(); // Snima ProductOrders
            ViewBag.OrderId = order.OrderID;
            return View("OrderSuccess");
        }

        [HttpPost]
        public async Task<IActionResult> SubmitRating(int orderId, double rating)
        {
            if (rating < 1 || rating > 5)
            {
                TempData["RatingError"] = "Invalid rating value.";
                return RedirectToAction("OrderSuccess");
            }

            var order = await _context.Orders.FindAsync(orderId);
            if (order == null)
            {
                return NotFound();
            }

            order.Rating = rating;
            await _context.SaveChangesAsync();


            return View("~/Views/Home/FinishRating.cshtml");
        }
        public IActionResult FinishRating()
        {
            return View();
        }
    }
}


