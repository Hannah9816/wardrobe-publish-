using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WardRobe.Models
{
    public class Trip
    {
        public int ID { get; set; }
        public string TripName { get; set; }
        public DateTime Date { get; set; }
        public string UserId { get; set; }
    }
}
