using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TiVerse.Application.DTO
{
    public class TopRoutesDTO
    {
        public string DeparturePoint {  get; set; }
        public string DestinationPoint {  get; set; }
        public int RouteFrequency { get; set; }
        public decimal MinPrice { get; set; }
    }
}
