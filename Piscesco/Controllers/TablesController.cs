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

        // creating an Feedback table
        private CloudTable GetFeedbackTableInformation()
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
            CloudTable table = clientAgent.GetTableReference("feedback");

            return table;
        }

        public void CreateFeedbackTable()
        {
            CloudTable table = GetFeedbackTableInformation();
            table.CreateIfNotExistsAsync();
        }

        // update the user's order_id
        [HttpPost]
        public async Task UpdateUserOrderID(string UserId, int OrderID)
        {
            var user = await _userManager.FindByIdAsync(UserId);

            user.OrderID = OrderID.ToString();
            await _userManager.UpdateAsync(user);
            await _signInManager.RefreshSignInAsync(user);
        }

    }
}
