using Cvjecara_Latica.Data;
using Cvjecara_Latica.Models;
using Cvjecara_Latica.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace Cvjecara_Latica.Controllers
{
    
    public class HomeController : Controller
    {
    private readonly ILogger<HomeController> _logger;
    private readonly UserManager<Person> _userManager;
    private readonly SignInManager<Person> _signInManager;
    private readonly ApplicationDbContext _context;
    private readonly EmailService _emailService;



        public HomeController(
        ILogger<HomeController> logger,
        UserManager<Person> userManager,
        SignInManager<Person> signInManager,
        ApplicationDbContext context,
        EmailService emailService)
    {
        _logger = logger;
        _userManager = userManager;
        _signInManager = signInManager;
        _context = context;
        _emailService = emailService;
        }
    public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }
        public IActionResult Login()
        {
            return View();
        }
        //novo
        [HttpPost]
        public async Task<IActionResult> Login(string email, string password)
        {
            var user = await _userManager.FindByEmailAsync(email);

            if (user == null)
            {
                ViewBag.Message = "This account does not exist.";
                return View();
            }

            if (!user.EmailConfirmed)
            {
                ViewBag.Message = "Please verify your email address before logging in.";
                return View();
            }
            var result = await _signInManager.PasswordSignInAsync(email, password, false, false);

            if (result.Succeeded)
            {
                return RedirectToAction("Index");
            }

            ViewBag.Message = "Invalid login attempt.";
            return View();
        }
        //dovde
        public IActionResult Register()
        {
            return View();
        }
        //novo
        [HttpPost]
        public async Task<IActionResult> Register(Register model)
        {
            var existingUser = await _userManager.FindByEmailAsync(model.Email);

            if (existingUser != null)
            {
                if (!existingUser.EmailConfirmed)
                {
                    // Resend code
                    var code = new Random().Next(100000, 999999).ToString();

                    TempData["VerificationCode"] = code;
                    TempData["UserEmail"] = existingUser.Email;
                    TempData["CodeGeneratedAt"] = DateTime.UtcNow;

                    TempData.Keep();

                    await _emailService.SendEmailAsync(
                        existingUser.Email,
                        "Email Verification Code",
                        $"<p>Your new verification code is: <strong>{code}</strong></p>"
                    );

                    ViewBag.Message = "This email is already registered but not verified. A new code has been sent.";
                    return RedirectToAction("VerifyCode");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "This email is already in use.");
                    return View(model);
                }
            }

            if (ModelState.IsValid)
            {
                var user = new Person
                {
                    UserName = model.Email,
                    Email = model.Email,
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    PhoneNumber=model.PhoneNumber,
                    HomeAdress = model.HomeAdress
                };

                var result = await _userManager.CreateAsync(user, model.Password);

                
                //dodano
                if (result.Succeeded)
                {
                    // Generiši 6-cifreni kod
                    var code = new Random().Next(100000, 999999).ToString();

                    // Privremeno èuvaj u sesiji
                    TempData["VerificationCode"] = code;
                    TempData["UserEmail"] = user.Email;
                    TempData["CodeGeneratedAt"] = DateTime.UtcNow;

                    TempData.Keep("VerificationCode");
                    TempData.Keep("UserEmail");
                    TempData.Keep("CodeGeneratedAt");


                    // Pošalji email
                    await _emailService.SendEmailAsync(
                        user.Email,
                        "Email Verification Code",
                        $"<p>Your verification code is: <strong>{code}</strong></p>"
                    );

                    return RedirectToAction("VerifyCode");
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            return View(model);
        }
        [HttpGet]
        public IActionResult VerifyCode()
        {
            TempData.Keep("VerificationCode");
            TempData.Keep("UserEmail");
            TempData.Keep("CodeGeneratedAt"); 
            return View();
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> VerifyCode(string code)

        {
            var storedCode = TempData["VerificationCode"]?.ToString();
            var email = TempData["UserEmail"]?.ToString();
            var codeTime = TempData["CodeGeneratedAt"] != null
          ? DateTime.Parse(TempData["CodeGeneratedAt"].ToString())
          : (DateTime?)null;

            TempData.Keep("VerificationCode");
            TempData.Keep("UserEmail");
            TempData.Keep("CodeGeneratedAt");

            if (storedCode == null || codeTime == null || email == null)
            {
                ViewBag.Error = "Code has expired. Please register again.";
                return View();
            }

            // Provjera da li je prošlo više od 2 minute
            if (DateTime.UtcNow - codeTime > TimeSpan.FromMinutes(2))
            {
                ViewBag.Error = "Code expired. Please request a new one.";
                return View();
            }

            if (code == storedCode)
            {
                var user = await _userManager.FindByEmailAsync(email);
                if (user != null && !user.EmailConfirmed)
                {
                    user.EmailConfirmed = true;
                    await _userManager.UpdateAsync(user);
                }

                // Dodaj popust korisniku nakon verifikacije
                var discount = new Discount
                {
                    DiscountCode = Guid.NewGuid().ToString().Substring(0, 8).ToUpper(),
                    DiscountAmount = 10,
                    DiscountType = DiscountType.PercentageOff,
                    DiscountBegins = DateTime.Now,
                    PersonID = user.Id,
                    IsUsed = false
                };

                _context.Discounts.Add(discount);
                await _context.SaveChangesAsync();

                // Pošalji kod korisniku na email
                await _emailService.SendEmailAsync(user.Email,
                    "Welcome! Here's your 10% discount code",
                    $"<p>Thank you for verifying your email. Use this code at checkout to get 10% off: <strong>{discount.DiscountCode}</strong></p>");


                return View("VerificationSuccess");
            }

            ViewBag.Error = "The code you entered is incorrect.";
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ResendCode()
        {
            var email = TempData["UserEmail"]?.ToString();
            if (email == null) return RedirectToAction("Register");

            var code = new Random().Next(100000, 999999).ToString();

            TempData["VerificationCode"] = code;
            TempData["UserEmail"] = email;
            TempData["CodeGeneratedAt"] = DateTime.UtcNow;

            await _emailService.SendEmailAsync(
                email,
                "New Verification Code",
                $"<p>Your new verification code is: <strong>{code}</strong></p>"
            );

            TempData.Keep(); // da se podaci zadrže još jedan request
            TempData["ResentMessage"] = "A new code has been sent to your email.";

            return RedirectToAction("VerifyCode");
        }




        public IActionResult Contact()
        {
            return View();
        }

        public IActionResult Help()
        {
            return View();
        }
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

       
        public IActionResult Search()
        {
            return View();
        }

        public IActionResult Filter(string type, string color, string category, string price)
        {
            // Ovdje kasnije dodaj pretragu iz baze, sada šaljemo samo View
            return View("FilteredResults");
        }
        [HttpGet]
        [HttpGet]
        public IActionResult SearchResults(string query, List<string> flowerTypes, List<string> colors, string category, string price)
        {
            var products = _context.Products.AsQueryable();

            if (!string.IsNullOrWhiteSpace(query))
            {
                products = products.Where(p =>
                    p.Name.Contains(query) ||
                    p.Description.Contains(query) ||
                    p.FlowerType.Contains(query) ||
                    p.Color.Contains(query));
            }

            if (flowerTypes != null && flowerTypes.Any())
            {
                products = products.Where(p => flowerTypes.Contains(p.FlowerType.ToLower()));
            }

            if (colors != null && colors.Any())
            {
                products = products.Where(p => colors.Contains(p.Color.ToLower()));
            }

            if (!string.IsNullOrWhiteSpace(category))
            {
                products = products.Where(p => p.Category.ToLower() == category.ToLower());
            }

            if (!string.IsNullOrWhiteSpace(price))
            {
                if (price.Contains("-"))
                {
                    var range = price.Split('-');
                    if (range.Length == 2 &&
                        int.TryParse(range[0].Trim(), out int min) &&
                        int.TryParse(range[1].Replace("BAM", "").Trim(), out int max))
                    {
                        products = products.Where(p => p.Price >= min && p.Price <= max);
                    }
                }
                else if (int.TryParse(price.Replace("BAM", "").Trim(), out int exact))
                {
                    products = products.Where(p => p.Price == exact);
                }
            }

            ViewBag.Query = query;
            ViewBag.FlowerTypes = flowerTypes;
            ViewBag.Colors = colors;
            ViewBag.Category = category;
            ViewBag.Price = price;

            return View(products.ToList());
        }


        public IActionResult BestSellers()
        {
            var bestSellers = _context.Products.Where(p => p.IsBestSeller).ToList();
            return View(bestSellers);
        }




    }
}
