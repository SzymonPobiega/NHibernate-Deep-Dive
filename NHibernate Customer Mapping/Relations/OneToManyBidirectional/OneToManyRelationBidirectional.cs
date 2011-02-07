using FluentAssertions;
using NHibernate_Customer_Mapping.Entities;
using NUnit.Framework;

namespace NHibernate_Deep_Dive.Relations.OneToMany
{
    [TestFixture]
    public class OneToManyRelationBidirectional: SpecificationBase
    {
        [Test]
        public void AddOrderToCustomer()
        {
            using (var session = SessionFactory.OpenSession())
            using (var transaction = session.BeginTransaction())
            {
                Customer customer = session.Get<Customer>(CustomerId);

                customer.Orders.Add(new Order() { Value = 3, Customer = customer });
                customer.Orders.Add(new Order() { Value = 5, Customer = customer });
                customer.Orders.Add(new Order() { Value = 13, Customer = customer });

                session.Save(customer);

                transaction.Commit();
            }

            using (var session = SessionFactory.OpenSession())
            {
                Customer customer = session.Get<Customer>(CustomerId);

                customer.Orders.Should().NotBeEmpty().And.HaveCount(x => x == 3);
            }
        }

        protected override string MappingsDirectory
        {
            get { return @"Relations\OneToManyBidirectional"; }
        }

        protected object CustomerId;


        [SetUp]
        public void PopulateDatabase()
        {
            using (var session = SessionFactory.OpenSession())
            using (var transaction = session.BeginTransaction())
            {
                CustomerId = session.Save(new Customer
                {
                    FirstName = "Terry",
                    LastName = "Gilliam"
                });

                transaction.Commit();
            }
        }
    }
}