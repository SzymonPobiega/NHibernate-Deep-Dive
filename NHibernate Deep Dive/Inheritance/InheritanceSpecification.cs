using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using NHibernate_Deep_Dive.Entities;
using NUnit.Framework;

namespace NHibernate_Deep_Dive.Inheritance
{
    public abstract class InheritanceSpecification : SpecificationBase
    {
        private object _preferredCustomerId;
        private object _bulkCustomerId;

        [Test]
        public void Polymorphic_Get()
        {
            using (var session = SessionFactory.OpenSession())
            {
                Customer anyCustomer;

                anyCustomer = session.Get<Customer>(_preferredCustomerId);

                anyCustomer.Should().NotBeNull();
                anyCustomer.Should().BeOfType<PreferredCustomer>();

                anyCustomer = session.Get<Customer>(_bulkCustomerId);

                anyCustomer.Should().NotBeNull();
                anyCustomer.Should().BeOfType<BulkCustomer>();
            }
        }

        [Test]
        public void Polymorphic_Criteria_Query()
        {
            using (var session = SessionFactory.OpenSession())
            {
                IEnumerable<Customer> customers = session.CreateCriteria<Customer>().List<Customer>();

                Customer preferredCustomer = customers.Single(x => x.Id.Equals(_preferredCustomerId));
                preferredCustomer.Should().BeOfType<PreferredCustomer>();

                Customer bulkCustomer = customers.Single(x => x.Id.Equals(_bulkCustomerId));
                bulkCustomer.Should().BeOfType<BulkCustomer>();
            }
        }

        [SetUp]
        public void PopulateDatabase()
        {
            using (var session = SessionFactory.OpenSession())
            using (var transaction = session.BeginTransaction())
            {
                _preferredCustomerId = session.Save(new PreferredCustomer
                                                       {
                                                           DiscountPercent = 10,
                                                           FirstName = "Terry",
                                                           LastName = "Gilliam"
                                                       });
                _bulkCustomerId = session.Save(new BulkCustomer
                                                  {
                                                      MinimumOrderValue = 1000,
                                                      FirstName = "Graham",
                                                      LastName = "Chapman"
                                                  });
                transaction.Commit();
            }
        }


    }
}