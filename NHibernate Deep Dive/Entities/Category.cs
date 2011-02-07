using System;
using System.Collections.Generic;

namespace NHibernate_Deep_Dive.Entities
{
    [Serializable]
    public class Category
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public IList<Order> Orders { get; set; }
    }
}