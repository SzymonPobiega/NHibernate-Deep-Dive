using System.Collections.Generic;
using System.Data.Services;
using System.Data.Services.Common;

namespace NHibernateDataService.Entities
{
    [DataServiceKey("Id")]
    public class Customer
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }

        public IList<Order> Orders { get; set; }

        public Customer()
        {
            Orders = new List<Order>();
        }
    }
}