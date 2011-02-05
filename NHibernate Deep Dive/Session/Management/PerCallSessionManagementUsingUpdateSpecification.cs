using FluentAssertions;
using NHibernate_Deep_Dive.Entities;
using NUnit.Framework;

namespace NHibernate_Deep_Dive.Session.Management
{
    public class PerCallSessionManagementUsingUpdateSpecification : SessionSpecification
    {
        [Test]
        public void Using_per_call_sessions_requires_with_Update_to_synchronize_changes()
        {
            HandleWebRequest();

            //Validate
            using (var session = SessionFactory.OpenSession())
            {
                var customer = session.Get<Customer>(FirstCustomerId);
                customer.FirstName.Should().Be("John");
            }
        }

        public void HandleWebRequest()
        {
            var customer = GetCustomerForDisplay();
            customer.FirstName = "John";
            UpdateCustomer(customer);
        }

        private void UpdateCustomer(Customer customer)
        {
            using (var session = SessionFactory.OpenSession())
            using (var transaction = session.BeginTransaction())
            {
                session.Update(customer);
                transaction.Commit();
            }
        }

        private Customer GetCustomerForDisplay()
        {
            using (var session = SessionFactory.OpenStatelessSession())
            {
                return session.Get<Customer>(FirstCustomerId);
            }
        }
    }
}