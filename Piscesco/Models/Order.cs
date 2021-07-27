using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Piscesco.Models
{
    public class Order
    {
        // defining the data columns of the order
        [Display(Name = "Order ID")]
        public int OrderID { get; set; }

        [Display(Name = "User ID")]
        // user_id related to the order
        public int UserID { get; set; }

        [Display(Name = "Status")]
        public string Status { get; set; }

        [Display(Name = "Total Price")]
        [Column(TypeName = "decimal(18,2)")]
        public decimal TotalPrice { get; set; }

        // date of confirming the transaction
        [Display(Name = "Date of Transaction")]
        public DateTime TransactionDate { get; set; }
    }
}
