using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TiVerse.Core.Entity
{
    public class Account
    {
        public Account(string userId, string firstName, string lastName, DateTime birthDate, string city, decimal cashBalance)
        {
            UserId = userId;
            FirstName = firstName;
            LastName = lastName;
            BirthDate = birthDate;
            City = city;
            CashBalance = cashBalance;
        }

        [Key]
        public string UserId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime BirthDate { get; set; }
        public string City { get; set; }
        public decimal CashBalance { get; set; }

        public virtual ICollection<UserRouteHistory> RouteHistory { get; set; }
    }
}
