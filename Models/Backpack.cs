using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace WardRobe.Models
{
    public class Backpack
    {
        public int Id { get; set; }

        [Display(Name = "Trip Name")]
        public string TripName { get; set; }

        [Display(Name = "Clothes to Bring")]
        public string Wardrobe { get; set; }
        public string UserId { get; set; }
    }
}
