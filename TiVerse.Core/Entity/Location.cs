using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TiVerse.Core.Entity
{
    internal class Location
    {
        public string City { get; set; }
        public string Country { get; set; }
        public bool BusStation { get; set; }
        public bool RailwayStation { get; set; }
        public bool Airport { get; set; }
    }
}
