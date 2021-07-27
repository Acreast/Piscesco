using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Piscesco.Models
{
    // extend to the table structure to allow it overwrite the content within the rowkey and partition key of table storage 
    public class FeedbackEntity:TableEntity
    {
        // assign the value to the partitionkey and rowkey
        public FeedbackEntity(string OrderID, string ProductID)
        {
            this.PartitionKey = OrderID; // one user can have many orders
            this.RowKey = ProductID; 
        }

        // if we don't wish to replace we can use an empty constructor
        public FeedbackEntity() { }

        // Date of confirmed the transaction of the order
        public string FeedbackContent { get; set; }
    }
}
