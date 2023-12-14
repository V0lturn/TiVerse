using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TiVerse.Core.Entity
{
    internal class UserRouteHistory
    {
        public Guid UserID { get; set; }
        public Guid TripID { get; set; }
        public string Route { get; set; }
        public string Transport { get; set; }
    }
}
