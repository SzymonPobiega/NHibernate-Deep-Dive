using System.Collections.Generic;
using System.Data.Services;
using System.Data.Services.Common;

namespace NHibernate_Customer_Mapping.Entities
{
    [IgnoreProperties("Address")]
    [DataServiceKey("Id")]
    public class Customer
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }

        public Address Address { get; set; }

        public IList<Order> Orders { get; set; }

        public Customer()
        {
            Orders = new List<Order>();
        }
    }
}