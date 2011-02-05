using System.Collections.Generic;

namespace NHibernate_Deep_Dive.Entities
{
    public class Order
    {
        public int Id { get; set; }
        public decimal Value { get; set; }
        public IList<Category> Categories { get; set; }

        public Order()
        {
            Categories = new List<Category>();
        }
    }
}