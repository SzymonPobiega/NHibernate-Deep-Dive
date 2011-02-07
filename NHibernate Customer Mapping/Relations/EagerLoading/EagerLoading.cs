using FluentAssertions;
using NHibernate;
using NHibernate_Customer_Mapping.Entities;
using NUnit.Framework;

namespace NHibernate_Deep_Dive.Relations.EagerLoading
{
    [TestFixture]
    public class EagerLoading: SpecificationBase
    {
        [Test]
        public void CheckEagerLoadingCheckingCollection()
        {
            using (var session = SessionFactory.OpenSession())
            {
                Customer customer = session.Get<Customer>(CustomerId);

                customer.Orders.Should().NotBeEmpty().And.HaveCount(x => x == 1);
            }
        }

        [Test]
        public void CheckEagerLoadingCheckingCollectionAfterSession()
        {
            Customer customer;
            using (var session = SessionFactory.OpenSession())
            {
                customer = session.Get<Customer>(CustomerId);
            }

            customer.Orders.Should().NotBeEmpty().And.HaveCount(x => x == 1);

        }

        protected override string MappingsDirectory
        {
            get { return @"Relations\EagerLoading"; }
        }

        protected object CustomerId;


        [SetUp]
        public void PopulateDatabase()
        {
            using (ISession session = SessionFactory.OpenSession())
            using (ITransaction transaction = session.BeginTransaction())
            {
                Customer customer = new Customer
                {
                    FirstName = "Terry",
                    LastName = "Gilliam"
                };

                customer.Orders.Add(new Order() { Value = 10, Customer = customer });

                CustomerId = session.Save(customer);


                transaction.Commit();
            }
        }
    }
}