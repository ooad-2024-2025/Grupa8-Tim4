using Cvjecara_Latica.Data;
using Cvjecara_Latica.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileSystemGlobbing.Internal;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using static Humanizer.On;
using Cvjecara_Latica.Patterns;
using Cvjecara_Latica.Paterni;


namespace Cvjecara_Latica.Controllers
{
    public class ProductController : Controller
    {
        private readonly ApplicationDbContext _context;
       // private readonly IProduct _productEditor;

        public ProductController(ApplicationDbContext context)
        {
            _context = context;
           // _productEditor = p;
        }

        // GET: Product
        public async Task<IActionResult> Index()
        {
            return View(await _context.Products.ToListAsync());
        }

        // GET: Product/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products
                .FirstOrDefaultAsync(m => m.ProductID == id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // GET: Product/Create
       
        public IActionResult Create()
        {
            return View();
        }
        public IActionResult SearchResult(string search)
        {
            List<Product> products = _context.Products.ToList();
            if (search == null)
                return View(products);
            else
            {
                string pattern = $"{Regex.Escape(search)}";
                List<Product> searchResults = products.Where(p => Regex.IsMatch(p.Name, pattern, RegexOptions.IgnoreCase)).ToList();

                ViewBag.String = search;
                return View(searchResults);
            }
        }

        public IActionResult PopularSearches(string popularsearch)
        {
            List<Product> products = _context.Products.ToList();
            bool isItBAM = Regex.IsMatch(popularsearch, "BAM", RegexOptions.IgnoreCase);
            if (isItBAM)
            {
                int o = int.Parse(popularsearch.Substring(4).Split(".").First());
                List<Product> inPriceRange = products.FindAll(p => p.Price <= o);
                ViewBag.p = inPriceRange;
            }
            else
            {
                List<Product> categoryList = _context.Products.ToList().FindAll(x => x.Category.ToLower() == popularsearch.ToLower());

                if (categoryList.Count == 0)
                {
                    categoryList = _context.Products.ToList().FindAll(x => x.FlowerType.ToLower() == popularsearch.ToLower());

                }
                ViewBag.String = popularsearch;
                ViewBag.p = categoryList;
            }
            return View("~/Views/Products/SearchResult.cshtml", ViewBag.p);
        }
        [HttpGet]
        public ActionResult Sort(string sortOption, string String)
        {
            ISort sortStrategy;

            if (sortOption == "ascendingName")
            {
                sortStrategy = new AscendingNameSortStrategy();
            }
            else if (sortOption == "descendingName")
            {
                sortStrategy = new DescendingNameSortStrategy();
            }
            else if (sortOption == "ascendingPrice")
            {
                sortStrategy = new AscendingPriceSortStrategy();
            }
            else if (sortOption == "descendingPrice")
                sortStrategy = new DescendingPriceSortStrategy();
            else
                sortStrategy = new AscendingNameSortStrategy();
            List<Product> searchResults;
            if (String != null)
            {
                searchResults = _context.Products.Where(x => x.Category == String).ToList();
                if (searchResults.Count() == 0)
                {
                    searchResults = _context.Products.Where(x => x.FlowerType == String).ToList();
                }
                if (searchResults.Count() == 0)
                {
                    string pattern = $"{Regex.Escape(String)}";
                    searchResults = _context.Products.ToList().Where(p => Regex.IsMatch(p.Name, pattern, RegexOptions.IgnoreCase)).ToList();
                }
            }

            else
                searchResults = _context.Products.ToList();

            ViewBag.String = String;
            var sortedProducts = sortStrategy.Sort(searchResults);
            ViewBag.SelectedSortOption = sortOption;

            return PartialView("~/Views/Products/SearchResult.cshtml", sortedProducts);
        }
        // POST: Product/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
       
        public async Task<IActionResult> Create([Bind("ProductID,Name,ImageUrl,Price,FlowerType,Stock,Category,Description,productType")] Product product)
        {
            if (ModelState.IsValid)
            {
                _context.Add(product);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(product);
        }

        // GET: Product/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            return View(product);
        }

        // POST: Product/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ProductID,Name,ImageUrl,Price,FlowerType,Stock,Category,Description,productType")] Product product)
        {
            if (id != product.ProductID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(product);
                    await _context.SaveChangesAsync();
                    //  _productEditor.EditAll(product);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProductExists(product.ProductID))
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
            return View(product);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        

        public async Task<IActionResult> EditNameAndPrice(int id, [Bind("ProductID,Name,Price")] Product product)
        {
            if (id != product.ProductID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(product);
                    await _context.SaveChangesAsync();
                    //  _productEditor.EditAll(product);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProductExists(product.ProductID))
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

            return View(product);
        }

        // GET: Product/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products
                .FirstOrDefaultAsync(m => m.ProductID == id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // POST: Product/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product != null)
            {
                _context.Products.Remove(product);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ProductExists(int id)
        {
            return _context.Products.Any(e => e.ProductID == id);
        }
    }
}
