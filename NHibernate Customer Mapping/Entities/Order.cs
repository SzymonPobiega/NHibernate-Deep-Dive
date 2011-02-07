using System.Collections.Generic;
using System.Data.Services;
using System.Data.Services.Common;

namespace NHibernate_Customer_Mapping.Entities
{
    [DataServiceKey("Id")]
    [IgnoreProperties("Categories")]
    public class Order
    {
        public int Id { get; set; }
        public decimal Value { get; set; }

        public Customer Customer { get; set; }

        public IList<Category> Categories { get; set; }

        public Order()
        {
            Categories = new List<Category>();
        }
    }
}