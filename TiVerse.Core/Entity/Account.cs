using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TiVerse.Core.Entity
{
    public class Account
    {
        public Account(Guid accountID, string firstName, string lastName, DateTime birthDate, string country, string city, decimal cashBalance)
        {
            AccountID = accountID;
            FirstName = firstName;
            LastName = lastName;
            BirthDate = birthDate;
            Country = country;
            City = city;
            CashBalance = cashBalance;
        }

        public Guid AccountID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime BirthDate { get; set; }
        public string Country { get; set; }
        public string City { get; set; }
        public decimal CashBalance { get; set; }
    }
}
