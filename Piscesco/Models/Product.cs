using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Piscesco.Models
{
    public class Product
    {
        public int ProductID { get; set; }

        public int StallID { get; set; }

        [Display(Name = "Product Name")]
        public string ProductName { get; set; }

        [Display(Name = "Description")]
        public string ProductDescription { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal Price { get; set; }

        [Display(Name = "Unit")]
        public string ProductUnit { get; set; }

        [Display(Name = "Product Image")]
        public string ProductImage { get; set; }
    }
}
