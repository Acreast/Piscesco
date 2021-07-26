using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

using Piscesco.Models;
using Microsoft.AspNetCore.Identity;
using Piscesco.Areas.Identity.Data;

namespace Piscesco.Controllers
{
    public class TablesController : Controller
    {
        private readonly UserManager<PiscescoUser> _userManager;
        private readonly SignInManager<PiscescoUser> _signInManager;

        public TablesController(UserManager<PiscescoUser> userManager, SignInManager<PiscescoUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        // creating an Order table
        private CloudTable GetOrderTableInformation()
        {
            // 1.1 link with the appsettings.json
            // import Microsoft.Extensions.Configuration
            // import System.IO;
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json");
            IConfiguration configure = builder.Build();

            //1.2 get the access connection string
            CloudStorageAccount accountDetails = CloudStorageAccount.Parse(configure["ConnectionStrings:BlobStorageConnection"]);

            //1.3 create client object to refer to the correct table
            CloudTableClient clientAgent = accountDetails.CreateCloudTableClient();
            CloudTable table = clientAgent.GetTableReference("order");

            return table;
        }

        public void CreateOrderTable()
        {
            CloudTable table = GetOrderTableInformation();
            table.CreateIfNotExistsAsync();
        }

        // creating an OrderList table
        private CloudTable GetOrderListTableInformation()
        {
            // 1.1 link with the appsettings.json
            // import Microsoft.Extensions.Configuration
            // import System.IO;
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json");
            IConfiguration configure = builder.Build();

            //1.2 get the access connection string
            CloudStorageAccount accountDetails = CloudStorageAccount.Parse(configure["ConnectionStrings:BlobStorageConnection"]);

            //1.3 create client object to refer to the correct table
            CloudTableClient clientAgent = accountDetails.CreateCloudTableClient();
            CloudTable table = clientAgent.GetTableReference("orderlist");

            return table;
        }

        public void CreateOrderListTable()
        {
            CloudTable table = GetOrderListTableInformation();
            table.CreateIfNotExistsAsync();
        }

        // generate a randomized unique alphanumeric string for order_id
        public static string RandomString()
        {
            var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            var stringChars = new char[8];
            var random = new Random();

            for (int i = 0; i < stringChars.Length; i++)
            {
                stringChars[i] = chars[random.Next(chars.Length)];
            }

            var finalString = new String(stringChars);

            return finalString;
        }

        // update the user's order_id
        [HttpPost]
        public async Task UpdateUserOrderID(string OrderID, string UserId)
        {
            var user = await _userManager.FindByIdAsync(UserId);
            user.OrderID = OrderID;
            await _userManager.UpdateAsync(user);
            await _signInManager.RefreshSignInAsync(user);
        }

        [HttpPost("{OrderID}/{ProductID}/{ProductQuantity}")]
        [ActionName("AddOrderItem")]
        // add new product item into the order list
        public IActionResult AddOrderItem(string OrderID, string ProductID, string ProductQuantity)
        {
            CreateOrderTable();
            CreateOrderListTable();

            // if the cart is empty or not having the same product ID then create a new one
            OrderListEntity orderlist = new OrderListEntity(OrderID, ProductID);
            orderlist.ProductQuantity = ProductQuantity;

            CloudTable table = GetOrderListTableInformation();
            try
            {
                TableOperation insertOperation = TableOperation.Insert(orderlist); // insertion action
                TableResult insertResult = table.ExecuteAsync(insertOperation).Result;
                ViewBag.result = insertResult.HttpStatusCode; // to get the network success code 204
                ViewBag.TableName = table.Name;
                ViewBag.Message = "Added into cart";
            }
            catch (Exception ex)
            {
                // show the error message if operation failed
                ViewBag.result = 100;
                ViewBag.Message = "Insertion Error: " + ex.ToString();
            }

            return RedirectToAction("BrowseProduct", "Products");
        }

        // update the existing order items if there already are an existing same product item within the order
        public bool UpdateOrderItem(string OrderID, string ProductID, string ProductQuantity)
        {
            bool status = false;

            // asdads

            return status;
        }

        // remove the specific order item upon request
        public bool RemoveOrderItem(string OrderID, string ProductID)
        {
            bool status = false;

            //adsdsdsdas

            return status;
        }

        // initialize the customer's empty order header into table storage
        public async Task InitializedOrderHeaderAsync(string user_id)
        {
            CreateOrderTable();
            CreateOrderListTable();

            // if the user have no ongoing status order, then automatically create for them
            // generate a unique order_id via randomized alphanumeric
            string order_id = RandomString();

            OrderEntity order = new OrderEntity(user_id, order_id);
            order.OrderStatus = "0"; // 0: Ongoing; 1: Confirmed
            order.TransactionDate = null;

            CloudTable table = GetOrderTableInformation();
            try
            {
                TableOperation insertOperation = TableOperation.Insert(order); // insertion action
                TableResult insertResult = table.ExecuteAsync(insertOperation).Result;
                // ViewBag.result = insertResult.HttpStatusCode; // to get the network success code 204
                // ViewBag.TableName = table.Name;

                // update the user table with the generated order_id
                await UpdateUserOrderID(order_id, user_id);
            }
            catch(Exception ex)
            {
                // show the error message if operation failed
                // ViewBag.result = 100;
                // ViewBag.Message = "Insertion Error: " + ex.ToString();
            }

            // else using the existing ongoing order's data
        }
    }

    // update the order's transaction date and status the moment the transaction has been confirmed


}
