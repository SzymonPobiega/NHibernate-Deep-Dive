using System;
using System.Collections.Generic;

namespace NHibernate_Deep_Dive.Entities
{
    [Serializable]
    public class Customer
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }

        public Address Address { get; set; }

        public byte[] DbVersion { get; set; }
        public int Version { get; set; }
        public DateTime Timestamp { get; set; }

        public IList<Order> Orders { get; set; }

        public Customer()
        {
            Orders = new List<Order>();
        }
    }
}