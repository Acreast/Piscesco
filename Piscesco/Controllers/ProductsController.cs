using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Piscesco.Areas.Identity.Data;
using Piscesco.Controllers;
using Piscesco.Data;
using Piscesco.Models;

namespace Piscesco.Views.Products
{
    public class ProductsController : Controller
    {
        private readonly PiscescoModelContext _context;
        private readonly UserManager<PiscescoUser> _userManager;
        private static int _stallID;



        public ProductsController(PiscescoModelContext context, UserManager<PiscescoUser> userManager)
        {
            _context = context;
            _userManager = userManager;


        }

        // GET: Products
        public async Task<IActionResult> Index(int? id)
        {
            if (id.HasValue)
            {
                //Debug.WriteLine(id);
                _stallID = (int)id;
            }
            var products = from p in _context.Product select p;
            products = products.Where(item => item.StallID.Equals(_stallID));


            return View(products);
        }

        // GET: Products
        public async Task<IActionResult> StallCatchSetup(int? id)
        {
            if (id.HasValue)
            {
                //Debug.WriteLine(id);
                _stallID = (int)id;
            }
            var products = from p in _context.Product select p;
            products = products.Where(item => item.StallID.Equals(_stallID));

            var featuredProducts = from p in _context.FeaturedProduct select p;
            featuredProducts = featuredProducts.Where(item => item.StallID.Equals(_stallID));

            List<Product> featuredProductsList = new List<Product>();
            foreach (var featuredItem in featuredProducts)
            {
                foreach (var productItem in products)
                {
                    if (featuredItem.ProductID.Equals(productItem.ProductID))
                    {
                        featuredProductsList.Add(productItem);
                    }
                }
            }

            //var featuredProducts = from p in _context.Product
            //                       join fp in _context.FeaturedProduct on p.ProductID equals fp.ProductID into result
            //                       where
            //                       p.StallID == _stallID 
            //                       select result;


            ViewData["Products"] = products;
            ViewData["FeaturedProducts"] = featuredProductsList;


            return View();
        }



        // GET: Products/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Product
                .FirstOrDefaultAsync(m => m.ProductID == id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }



        // GET: Products/Create
        public IActionResult Create()
        {
            Product p = new Product();
            p.StallID = _stallID;
            return View(p);
        }

        // POST: Products/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ProductID,StallID,ProductName,ProductDescription,Price,ProductUnit,Stock,ProductImage")] Product product, IFormFile files)
        {
            if (ModelState.IsValid)
            {
                //If stall does no exist
                var stall = await _context.Stall.FindAsync(_stallID);
                if (stall == null)
                {
                    return NotFound();
                }

                //Set stall ID
                product.StallID = _stallID;

                //Image upload
                Guid imageUUID = Guid.NewGuid();
                string imageUUIDString = imageUUID.ToString();
                product.ProductImage = imageUUIDString;
                BlobsController bc = new BlobsController();
                bc.UploadImage(files, imageUUIDString);

                _context.Add(product);



                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(product);
        }

        // GET: Products/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Product.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            return View(product);
        }

        // POST: Products/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ProductID,StallID,ProductName,ProductDescription,Price,ProductUnit,Stock,ProductImage")] Product product, IFormFile files)
        {
            if (id != product.ProductID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    if (files != null)
                    {
                        Guid imageUUID = Guid.NewGuid();
                        string imageUUIDString = imageUUID.ToString();
                        product.ProductImage = imageUUIDString;
                        BlobsController bc = new BlobsController();
                        bc.UploadImage(files, imageUUIDString);

                    }

                    _context.Update(product);
                    await _context.SaveChangesAsync();
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
        [ActionName("AddFeatured")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddFeatured(int id, [Bind("FeaturedProductID,StallID,ProductID")] FeaturedProduct fp)
        {
            if (ModelState.IsValid)
            {
                //If stall does no exist
                var stall = await _context.Stall.FindAsync(_stallID);
                if (stall == null)
                {
                    return NotFound();
                }

                //Set stall ID
                fp.StallID = _stallID;
                fp.ProductID = id;
                _context.Add(fp);

                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(StallCatchSetup));
        }

        // GET: Products/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Product
                .FirstOrDefaultAsync(m => m.ProductID == id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // POST: Products/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var product = await _context.Product.FindAsync(id);
            _context.Product.Remove(product);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // POST: Products/Delete/5
        [HttpPost, ActionName("RemoveFeatured")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RemoveFeatured(int id)
        {
            //var product = await _context.Product.FindAsync(id);
            var featuredProducts = from p in _context.FeaturedProduct
                                   select p;
            FeaturedProduct selectedProduct = new FeaturedProduct();
            foreach(var fp in featuredProducts)
            {
                if (fp.ProductID.Equals(id) && fp.StallID.Equals(_stallID))
                {
                    selectedProduct = fp;
                }
            }
            //var delete = await _context.FeaturedProduct.FindAsync(selectedProduct.FeaturedProductID);
            _context.FeaturedProduct.Remove(selectedProduct);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(StallCatchSetup));
        }

        private bool ProductExists(int id)
        {
            return _context.Product.Any(e => e.ProductID == id);
        }
    }
}
