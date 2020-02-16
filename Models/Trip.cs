using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace WardRobe.Models
{
    public class Trip
    {
        public int ID { get; set; }

        [Display(Name = "Trip Name")]
        public string TripName { get; set; }
        public DateTime Date { get; set; }
        public string UserId { get; set; }
    }
}
