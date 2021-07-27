using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Piscesco.Models
{
    public class OrderList
    {
        // defining the data columns of the orderlist items
        [Display(Name = "Order list ID")]
        public int OrderListID { get; set; }

        [Display(Name = "Product ID")]
        public string ProductID { get; set; }

        [Display(Name = "Product Image")]
        public string ProductImage { get; set; }

        [Display(Name = "Product Name")]
        public string ProductName { get; set; }

        [Display(Name = "Product Quantity")]
        [Column(TypeName = "decimal(18,0)")]
        public int ProductQuantity { get; set; }

        [Display(Name = "Total Price (RM)")]
        [Column(TypeName = "decimal(18,2)")]
        public decimal TotalPrice { get; set; }

        // order_id related to the order (FKey)
        [Display(Name = "Order ID")]
        public int OrderID { get; set; }
    }
}
