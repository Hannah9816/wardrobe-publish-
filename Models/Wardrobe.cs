using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace WardRobe.Models
{
    public class Wardrobe
    {
        public int ID { get; set; }
        public string Category { get; set; }
        public string Name { get; set; }
        public string Brand { get; set; }

        [Display(Name = "Purchased Date")]
        [DataType(DataType.Date)]
        public DateTime PurchaseDate { get; set; }

        [Range(1, 100)]
        [DataType(DataType.Currency)]
        [Column(TypeName = "decimal(18,3)")]
        [Display(Name = "Purchased Price")]
        public decimal Price { get; set; }

        [Display(Name = "Worn Times")]
        public int WornTimes { get; set; }

        [Display(Name = "Image")]
        public string ImageUrl { get; set; }

        public string FileName { get; set; }
        public string UserId { get; set; }

    }
}
