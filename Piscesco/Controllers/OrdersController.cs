using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Piscesco.Areas.Identity.Data;
using Piscesco.Data;
using Piscesco.Models;

namespace Piscesco.Controllers
{
    public class OrdersController : Controller
    {
        private readonly PiscescoModelContext _context;
        private readonly UserManager<PiscescoUser> _userManager;
        private static int _stallID;
        private static DateTime _startingDate;
        private static DateTime _endDate;

        public OrdersController(PiscescoModelContext context, UserManager<PiscescoUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }


        // GET: Stall all orders
        public async Task<IActionResult> OrderByStall(int? id)
        {

            if (id.HasValue)
            {
                _stallID = (int)id;
            }
            var products = from p in _context.Product select p;
            products = products.Where(item => item.StallID.Equals(_stallID));
            
            ViewData["Products"] = products;

            var orderList = await _context.Order.Where(orderItem => orderItem.StallID == _stallID && orderItem.Status == "Pending").ToListAsync();

            return View(orderList);
        }

        // GET: Stall orders by date
        public async Task<IActionResult> OrderByDate(string start, DateTime? date)
        {
            //Debug.WriteLine((DateTime)date);

            if (!String.IsNullOrEmpty(start))
            {
                _startingDate = Convert.ToDateTime(start);
            }

            if (date.HasValue)
            {
                _startingDate = (DateTime)date;
            }
            var products = from p in _context.Product select p;
            products = products.Where(item => item.StallID.Equals(_stallID));

            ViewData["Products"] = products;

            //var orderList = await _context.Order.Where(orderItem => orderItem.StallID == _stallID && orderItem.Status == "Pending").ToListAsync();

            var orderList =  _context.Order.
                FromSqlRaw("SELECT * FROM [Order] WHERE TransactionDate >= " + "'"+_startingDate.ToString("yyyy-MM-dd")+"'" + "AND TransactionDate <= '" +_startingDate.AddDays(1).AddTicks(-1) + "'")
                .ToList();
            ViewData["DateOfOrder"] = _startingDate.ToString("yyyy-MM-dd");

            return View(orderList);
        }

        //Get orders by date
        public PartialViewResult SearchByDate(string start)
        {
            Debug.WriteLine(start);
            if (!String.IsNullOrEmpty(start))
            {
                _startingDate = Convert.ToDateTime(start);
            }

            var products = from p in _context.Product select p;
            products = products.Where(item => item.StallID.Equals(_stallID));

            ViewData["Products"] = products;

            var orderList = _context.Order.
                FromSqlRaw("SELECT * FROM [Order] WHERE TransactionDate >= " + "'" + _startingDate.ToString("yyyy-MM-dd") + "'" + "AND TransactionDate <= '" + _startingDate.AddDays(1).AddTicks(-1) + "'")
                .ToList();
            ViewData["DateOfOrder"] = _startingDate.ToString("yyyy-MM-dd");

            return PartialView("_OrderPartialView",orderList);
        }


        public async Task<IActionResult> StallList()
        {
            var loginSession = await _userManager.FindByIdAsync(_userManager.GetUserId(User));
            var stalls = from s in _context.Stall select s;
            stalls = stalls.Where(item => item.OwnerID.Equals(loginSession.Id));

            return View(await stalls.ToListAsync());
        }

        // GET: Orders/Details/5
        public async Task<IActionResult> Details(Guid? id, int? pid)
        {
            if (id == null)
            {
                return NotFound();
            }

            var order = await _context.Order
                .FirstOrDefaultAsync(m => m.OrderID == id);
            if (order == null)
            {
                return NotFound();
            }
            if(pid == null)
            {
                return NotFound();
            }
            var product = await _context.Product.FindAsync(pid);
            ViewData["Product"] = product;

            return View(order);
        }

        // GET: Orders/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Orders/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("OrderID,StallID,ProductID,ProductName,ProductQuantity,TotalPrice,Status,TransactionDate")] Order order)
        {
            if (ModelState.IsValid)
            {
                order.OrderID = Guid.NewGuid();
                _context.Add(order);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(order);
        }

        // GET: Orders/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var order = await _context.Order.FindAsync(id);
            if (order == null)
            {
                return NotFound();
            }
            return View(order);
        }

        // POST: Orders/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("OrderID,StallID,ProductID,ProductName,ProductQuantity,TotalPrice,Status,TransactionDate")] Order order)
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
            return View(order);
        }

        // GET: Orders/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var order = await _context.Order
                .FirstOrDefaultAsync(m => m.OrderID == id);
            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }

        // POST: Orders/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var order = await _context.Order.FindAsync(id);
            _context.Order.Remove(order);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }






        private bool OrderExists(Guid id)
        {
            return _context.Order.Any(e => e.OrderID == id);
        }
    }
}
