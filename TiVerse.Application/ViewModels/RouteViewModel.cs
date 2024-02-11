using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TiVerse.Application.ViewModels
{
    public class RouteViewModel
    {
        public string DeparturePoint { get; set; }
        public string DestinationPoint { get; set; }
        public DateTime DateOfTrip { get; set; }
        public string Transport { get; set; }
        public string Company { get; set; }
        public int Places { get; set; }
        public decimal TicketCost { get; set; }
    }
}
