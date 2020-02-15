using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage.Table;
namespace WardRobe.Models
{
    public class Haul: TableEntity
    {
        public Haul(string cat, string haul)
        {
            this.PartitionKey = cat;
            this.RowKey = haul;
        }
        public Haul() { }
        public string category { get; set; }
        public string clothes { get; set; }
        public string brand { get; set; }
        public string description { get; set; }
        public string UserId { get; set; }

    }
}
