using AutoMapper;

namespace TiVerse.Application.DTO
{
    public class RouteDTO
    {
        public Guid TripID { get; set; }
        public string DeparturePoint { get; set; }
        public string DestinationPoint { get; set; }
        public DateTime Date { get; set; }
        public int Places { get; set; }
        public decimal TicketCost { get; set; }
    }
}
