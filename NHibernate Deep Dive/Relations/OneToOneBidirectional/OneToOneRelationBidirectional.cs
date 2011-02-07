using FluentAssertions;
using NHibernate_Deep_Dive.Entities;
using NUnit.Framework;

namespace NHibernate_Deep_Dive.Relations.OneToOneBidirectional
{
    [TestFixture]
    public class OneToOneRelationBidirectional: SpecificationBase
    {
        [Test]
        public void AddCustomerToAddress()
        {
            object addressId;
            using (var session = SessionFactory.OpenSession())
            using (var transaction = session.BeginTransaction())
            {
                Customer customer = new Customer() { FirstName = "Name", LastName = "LastName" };
                Address address = new Address()
                {
                    City = "Cracow",
                    Country = "Poland",
                    PostalCode = "30-133",
                    Street = "Lea",
                    StreetNumber = "116"
                };

                address.Customer = customer;

                addressId = session.Save(address);

                transaction.Commit();
            }

            using (var session = SessionFactory.OpenSession())
            {
                Address addressSecond = session.Get<Address>(addressId);

                addressSecond.Customer.Should().NotBeNull();
                Customer customerSecond = session.Get<Customer>(addressSecond.Customer.Id);

                customerSecond.Address.Should().NotBeNull();
            }
        }
        
        protected override string MappingsDirectory
        {
            get { return @"Relations\OneToOneBidirectional"; }
        }
    }
}