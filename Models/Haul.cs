﻿using System.ComponentModel.DataAnnotations;
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

        [Display(Name = "Category")]
        public string category { get; set; }

        [Display(Name = "Name")]
        public string clothes { get; set; }

        [Display(Name = "Brand")]
        public string brand { get; set; }

        [Display(Name = "Remarks")]
        public string description { get; set; }
        public string UserId { get; set; }

    }
}
