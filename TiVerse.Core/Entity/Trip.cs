using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TiVerse.Core.Entity
{
    public class Trip
    {
        public Trip() 
        {
            DeparturePoint = string.Empty;
            DestinationPoint = string.Empty;
            Transport = string.Empty;
            Company = string.Empty;
        }

        public Trip (string departurePoint, string destinationPoint, DateTime date, string transport, string company, int places, decimal ticketCost)
        {
            TripID = Guid.NewGuid();
            DeparturePoint = departurePoint;
            DestinationPoint = destinationPoint;
            Date = date;
            Transport = transport;
            Company = company;
            Places = places;
            TicketCost = ticketCost;
        }

        public Guid TripID { get; set; }
        public string DeparturePoint { get; set; }
        public string DestinationPoint { get; set; }
        public DateTime Date { get; set; }
        public string Transport { get; set; }
        public string Company { get; set; }
        public int Places { get; set; }
        public decimal TicketCost { get; set; }
    }
}
