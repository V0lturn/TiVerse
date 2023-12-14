using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TiVerse.Core.Entity
{
    internal class User
    {
        public Guid UserID { get; set; }

        public string FirstName {get; set; }
        public string LastName { get; set; }
        public DateTime BirthDate { get; set; }
        public string Country { get; set; }
        public string City { get; set; }
        public decimal AccountCashBalance { get; set; }
    }
}
