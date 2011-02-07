using System;
using System.Collections;
using System.Collections.Generic;
using NHibernate;
using NHibernate.Criterion;
using NHibernate_Customer_Mapping.Entities;
using NUnit.Framework;
using Order = NHibernate_Customer_Mapping.Entities.Order;

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

                IList<Customer> customers =
                    session.CreateCriteria(typeof (Customer)).CreateCriteria("Orders", "o").Add(
                        Expression.Gt("o.Value", 90m)).List<Customer>();

                IList results =
                    session.CreateCriteria<Customer>().CreateCriteria("Orders", "o").SetProjection(Projections.RowCount()).SetProjection(
                        Projections.GroupProperty("LastName")).SetProjection(Projections.Max("o.Value")).List();

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

        [SetUp]
        public void PrepareDatabase()
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