using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TiVerse.Core.Entity
{
    public class UserRouteHistory
    {
        [Key]
        public int UserRouteHistoryId { get; set; }

        [ForeignKey("UserId")]
        public string UserId { get; set; }

        [ForeignKey("TripID")]
        public Guid TripID { get; set; }

        public DateTime Date { get; set; }

        public virtual Account User { get; set; }
        public virtual Trip Trip { get; set; }

        public UserRouteHistory()
        { 

        }

        public UserRouteHistory(string userId, Guid tripId, DateTime date)
        {
            UserId = userId;
            TripID = tripId;
            Date = date;
        }
    }
}
