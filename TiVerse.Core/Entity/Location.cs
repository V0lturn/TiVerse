using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TiVerse.Core.Entity
{
    public class Location
    {
        [Key]
        public string City { get; set; }
        public string Country { get; set; }
        public bool BusStation { get; set; }
        public bool RailwayStation { get; set; }
        public bool Airport { get; set; }

        public Location(string city, string country, bool busStation, bool railwayStation, bool airport) 
        {
            City = city;
            Country = country;
            BusStation = busStation;
            RailwayStation = railwayStation;
            Airport = airport;
        }
    }
}
