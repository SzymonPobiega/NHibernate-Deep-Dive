using System;
using System.Collections;
using System.Collections.Generic;
using NHibernate;
using NHibernate_Customer_Mapping.Entities;
using NUnit.Framework;

namespace NHibernate_Deep_Dive.FetchingData.HQL
{
    [TestFixture]
    public class HQLQuery: SpecificationBase
    {
        [Test]
        public void SimpleQueries()
        {
            using(ISession session = SessionFactory.OpenSession())
            {
                long result = (long) session.CreateQuery("select count(*) from Customer").UniqueResult();
                IList<Customer> customers =
                    session.CreateQuery("select cust from Customer as cust inner join cust.Orders as order where order.Value > 90").List
                        <Customer>();

                IList results = session.CreateQuery(
                    "select cust.LastName, max(o.Value) from Customer as cust inner join cust.Orders as o group by cust.LastName").List();

                Assert.AreEqual(10, result);
            }

        }

        protected override string MappingsDirectory
        {
            get { return @"FetchingData\HQL"; }
        }

        [SetUp]
        public void PrepairDB()
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
                        customer.Orders.Add(new Order() {Value = i, Customer = customer});
                    }

                    session.Save(customer);
                }

                transaction.Commit();
            }
        }
    }
}