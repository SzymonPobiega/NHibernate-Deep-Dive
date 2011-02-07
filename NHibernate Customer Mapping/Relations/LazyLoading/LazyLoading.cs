using FluentAssertions;
using NHibernate;
using NHibernate_Customer_Mapping.Entities;
using NUnit.Framework;

namespace NHibernate_Deep_Dive.Relations.LazyLoading
{
    [TestFixture]
    public class LazyLoading: SpecificationBase
    {
        [Test]
        public void CheckLazyLoadingCheckingCollection()
        {
            using (var session = SessionFactory.OpenSession())
            {
                Customer customer = session.Get<Customer>(CustomerId);

                customer.Orders.Should().NotBeEmpty().And.HaveCount(x => x == 1);
            }
        }

        [Test]
        public void CheckLazyLoadingCheckingCollectionAfterInitialization()
        {
            Customer customer;
            using (var session = SessionFactory.OpenSession())
            {
                customer = session.Get<Customer>(CustomerId);
                NHibernateUtil.Initialize(customer.Orders);
                
            }

            customer.Orders.Should().NotBeEmpty().And.HaveCount(x => x == 1);
        }

        [Test]
        public void CheckLazyLoadingCheckingCollectionAfterSession()
        {
            Customer customer;
            using (var session = SessionFactory.OpenSession())
            {
                customer = session.Get<Customer>(CustomerId);
            }

            Assert.IsFalse(NHibernateUtil.IsInitialized(customer.Orders));
        }

        protected override string MappingsDirectory
        {
            get { return @"Relations\LazyLoading"; }
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

                customer.Orders.Add(new Order() {Value = 10, Customer = customer});

                CustomerId = session.Save(customer);


                transaction.Commit();
            }
        }
    }
}