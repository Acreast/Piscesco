using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Piscesco.Models
{
    // extend to the table structure to allow it overwrite the content within the rowkey and partition key of table storage 
    public class OrderListEntity:TableEntity
    {
        // assign the value to the partitionkey and rowkey
        public OrderListEntity(string OrderID, string ProductID)
        {
            this.PartitionKey = OrderID; // one order can have many product
            this.RowKey = ProductID;
        }

        // if we don't wish to replace we can use an empty constructor
        public OrderListEntity() { }

        // defining the specific content that each rowkey will have
        // quantity of the specific product
        public string ProductQuantity { get; set; }
    }
}
