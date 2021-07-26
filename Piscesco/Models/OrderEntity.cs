using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Piscesco.Models
{
    // extend to the table structure to allow it overwrite the content within the rowkey and partition key of table storage 
    public class OrderEntity:TableEntity
    {
        // assign the value to the partitionkey and rowkey
        public OrderEntity(string UserID, string OrderID)
        {
            this.PartitionKey = UserID; // one user can have many orders
            this.RowKey = OrderID; 
        }

        // if we don't wish to replace we can use an empty constructor
        public OrderEntity() { }

        // defining the specific content that each rowkey will have
        // 0: Ongoing; 1: Confirmed
        public string OrderStatus { get; set; }

        // Date of confirmed the transaction of the order
        public string TransactionDate { get; set; }
    }
}
