using System.Collections.Generic;
using System.Data.Services;
using System.Data.Services.Common;

namespace NHibernateDataService.Entities
{
    [DataServiceKey("Id")]
    public class Order
    {
        public int Id { get; set; }
        public decimal Value { get; set; }

        public Customer Customer { get; set; }
    }
}