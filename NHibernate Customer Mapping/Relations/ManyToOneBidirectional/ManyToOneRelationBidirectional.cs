using FluentAssertions;
using NHibernate_Customer_Mapping.Entities;
using NHibernate_Deep_Dive;
using NUnit.Framework;

namespace NHibernate_Customer_Mapping.Relations.ManyToOne
{
    [TestFixture]
    public class ManyToOneRelationBidirectional : SpecificationBase
    {
        [Test]
        public void Customer_Address_OneWayRelation()
        {
            using (var session = SessionFactory.OpenSession())
            using (var transaction = session.BeginTransaction())
            {
                Customer customer = session.Get<Customer>(CustomerId);
                Address address = session.Get<Address>(AddressId);

                customer.Address = address;

                session.Save(customer);

                transaction.Commit();
            }

            using (var session = SessionFactory.OpenSession())
            {
                Customer customerSecond = session.Get<Customer>(CustomerId);

                customerSecond.Address.Should().NotBeNull();
            }
        }

        [Test]
        public void Customer_Address_TwoWayRelation()
        {
            using (var session = SessionFactory.OpenSession())
            using (var transaction = session.BeginTransaction())
            {
                Customer customer = session.Get<Customer>(CustomerId);
                Address address = session.Get<Address>(AddressId);

                customer.Address = address;

                session.Save(customer);

                transaction.Commit();
            }

            using (var session = SessionFactory.OpenSession())
            {
                Customer customerSecond = session.Get<Customer>(CustomerId);
                Address addressSecond = session.Get<Address>(AddressId);

                addressSecond.Customer.Should().NotBeNull();
                customerSecond.Address.Should().NotBeNull();
            }

        }

        [Test]
        public void Customer_Address_TwoWayRelationAssignCustomerToAddress()
        {
            using (var session = SessionFactory.OpenSession())
            using (var transaction = session.BeginTransaction())
            {
                Customer customer = session.Get<Customer>(CustomerId);
                Address address = session.Get<Address>(AddressId);

                customer.Address = address;
                address.Customer = customer;

                session.Save(customer);

                transaction.Commit();
            }

            using (var session = SessionFactory.OpenSession())
            {
                Customer customerSecond = session.Get<Customer>(CustomerId);
                Address addressSecond = session.Get<Address>(AddressId);

                addressSecond.Customer.Should().NotBeNull();
                customerSecond.Address.Should().NotBeNull();
            }

        }

        protected override string MappingsDirectory
        {
            get { return @"Relations\ManyToOneBidirectional"; }
        }

        protected object CustomerId;
        protected object AddressId;


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

                AddressId = session.Save(new Address()
                                             {
                                                 City = "Cracow",
                                                 Country = "Poland",
                                                 PostalCode = "30-133",
                                                 Street = "Lea",
                                                 StreetNumber = "116"
                                             });
                transaction.Commit();
            }
        }
    }
}
