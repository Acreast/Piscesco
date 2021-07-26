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
        public async Task<IActionResult> BrowseProduct(String ProductName, String StallID)
        {
            // technically just like SQL query
            var product = from m in _context.Product join x in _context.Stall
                          on m.StallID equals x.StallID
                          select m; // selecting the product/data from the context product

            // to check whether its there or not
            if (!string.IsNullOrEmpty(ProductName))
            {
                // s stands for database variable, ProductName is the table column
                product = product.Where(s => s.ProductName.Contains(ProductName));
            }

            // to show and attach the available product types in the dropdown list from Index.cshtml
            // m as in pointing at the Product table
            // getting all the available FlowerType data from the respective table
            IQueryable<String> TypeQuery = from m in _context.Product join x in _context.Stall
                                           on m.StallID equals x.StallID
                                           orderby m.StallID
                                           select m.StallID.ToString();
            // find the disctinct value, aka removing the duplicated values and attach them as a list
            IEnumerable<SelectListItem> items =
                new SelectList(await TypeQuery.Distinct().ToListAsync());
            // using viewbag to attach it on the front-end, which is the drop down box in this case.
            ViewBag.StallID = items;

            // after attach the available StaffName value in the dropdown list, we will proceed to code the feature for filteration
            if (!string.IsNullOrEmpty(StallID))
            {
                // s stands for database variable, StallName is the table column
                product = product.Where(s => s.StallID.ToString() == StallID);
            }

            // Default: this is to display the entire page/data on load, we will change it to only show after filter
            // return View(await _context.Product.ToListAsync());
            return View(await product.ToListAsync());
        }

        // GET: Products
        public async Task<IActionResult> Index(int? id)
        {
            if (id.HasValue)
            {
                Debug.WriteLine(id);
                _stallID = (int)id;
            }
            var products = from p in _context.Product select p;
            products = products.Where(item => item.StallID.Equals(_stallID));


            return View(products);
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
        public async Task<IActionResult> Create([Bind("ProductID,StallID,ProductName,ProductDescription,Price,ProductUnit,ProductImage")] Product product, IFormFile files)
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
        public async Task<IActionResult> Edit(int id, [Bind("ProductID,StallID,ProductName,ProductDescription,Price,ProductUnit,ProductImage")] Product product, IFormFile files)
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

        private bool ProductExists(int id)
        {
            return _context.Product.Any(e => e.ProductID == id);
        }
    }
}
