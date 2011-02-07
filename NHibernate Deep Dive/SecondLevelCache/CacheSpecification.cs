using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using log4net.Config;
using NHibernate_Deep_Dive.Entities;
using NUnit.Framework;

namespace NHibernate_Deep_Dive.SecondLevelCache
{
    public abstract class CacheSpecification : SpecificationBase
    {
        protected object OrderId;
        protected object FirstCategoryId;
        protected object SecondCategoryId;


        protected override void BeforeTestRun()
        {

            using (var session = SessionFactory.OpenSession())
            using (var transaction = session.BeginTransaction())
            {
                var firstCategory = new Category
                                        {
                                            Name = "First"
                                        };
                var secondCategory = new Category
                                         {
                                             Name = "Second"
                                         };

                FirstCategoryId = session.Save(firstCategory);
                SecondCategoryId = session.Save(secondCategory);

                var order = new Order
                                {
                                    Value = 10,
                                };
                order.Categories.Add(firstCategory);
                order.Categories.Add(secondCategory);
                OrderId = session.Save(order);
                transaction.Commit();
            }
        }
    }
}