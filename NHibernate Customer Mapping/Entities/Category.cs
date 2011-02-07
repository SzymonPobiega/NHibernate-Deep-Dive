using System.Collections.Generic;

namespace NHibernate_Customer_Mapping.Entities
{
    public class Category
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public IList<Order> Orders { get; set; }
    }
}