using System.ComponentModel.DataAnnotations;

namespace TiVerse.Core.Entity
{
    public class UserRouteHistory
    {
        [Key]
        public Guid UserID { get; set; }
        public Guid TripID { get; set; }
        public string Route { get; set; }
        public string Transport { get; set; }
    }
}
