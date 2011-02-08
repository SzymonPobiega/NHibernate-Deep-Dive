using System;
using System.Collections;
using System.Collections.Generic;
using FluentAssertions;
using NHibernate;
using NHibernate.Criterion;
using NHibernate_Deep_Dive.Entities;
using NUnit.Framework;
using Order = NHibernate_Deep_Dive.Entities.Order;

namespace NHibernate_Deep_Dive.FetchingData.Criteria
{
    [TestFixture]
    public class CriteriaQuery: SpecificationBase
    {
        [Test]
        public void SimpleQueries()
        {
            using(ISession session = SessionFactory.OpenSession())
            {
                int result = (int) session.CreateCriteria(typeof(Customer)).SetProjection(Projections.RowCount()).UniqueResult();

                session.CreateCriteria(typeof (Customer))
                    .CreateCriteria("Orders", "o")
                    .Add(Restrictions.Gt("o.Value", 90m))
                    .List<Customer>();

                session.CreateCriteria<Customer>()
                    .CreateCriteria("Orders", "o")
                    .SetProjection(Projections.RowCount())
                    .SetProjection(Projections.GroupProperty("LastName"))
                    .SetProjection(Projections.Max("o.Value"))
                    .List();

                foreach (var customer in session.CreateCriteria<Customer>().List<Customer>())
                {
                    var orderCound = customer.Orders.Count;
                }

                Assert.AreEqual(10, result);
            }

        }

        [Test]
        public void QueryOver()
        {
            using (ISession session = SessionFactory.OpenSession())
            {
                int result = session.QueryOver<Customer>().RowCount();

                IList<Customer> customers =
                    session.QueryOver<Customer>().JoinQueryOver<Order>(cust => cust.Orders).Where(
                        o => o.Value > 90).List<Customer>();

                IList<object> results =
                    session.QueryOver<Customer>()
                    .SelectList(list => 
                        list.SelectGroup(c => c.LastName))
                    .JoinQueryOver<Order>(cust => cust.Orders).List<object>();

                Assert.AreEqual(10, result);
            }

        }

        protected override string MappingsDirectory
        {
            get { return @"FetchingData\Criteria"; }
        }

        protected override void  PopulateDatabase()
        {
            using(ISession session = SessionFactory.OpenSession())
            using(ITransaction transaction = session.BeginTransaction())
            {
                for(int i = 0; i < 10; i++)
                {
                    Customer customer = new Customer
                                            {
                                                FirstName = "Terry" + i.ToString(),
                                                LastName = "Gilliam" + i.ToString()
                                            };
                    for (int j = 10 - i; j > 0; j--)
                    {
                        customer.Orders.Add(new Order() {Value = j, Customer = customer});
                    }

                    session.Save(customer);
                }

                transaction.Commit();
            }
        }
    }
}