using FluentAssertions;
using NHibernate_Deep_Dive.Entities;
using NUnit.Framework;

namespace NHibernate_Deep_Dive.Session.Management
{
    public class PerCallSessionManagementUsingDTOSpecification : SessionSpecification
    {
        public class CustomerDTO
        {
            public int Id { get; set; }
            public string FirstName { get; set; }
            public string LastName { get; set; }
        }

        [Test]
        public void Using_per_call_sessions_with_DTO()
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
            var customerDTO = GetCustomerForDisplay();
            customerDTO.FirstName = "John";
            UpdateCustomer(customerDTO);
        }

        private void UpdateCustomer(CustomerDTO customerDTO)
        {
            using (var session = SessionFactory.OpenSession())
            using (var transaction = session.BeginTransaction())
            {
                var customer = session.Get<Customer>(customerDTO.Id);
                customer.FirstName = customerDTO.FirstName;
                customer.LastName = customerDTO.LastName;
                transaction.Commit();
            }
        }

        private CustomerDTO GetCustomerForDisplay()
        {
            using (var session = SessionFactory.OpenStatelessSession())
            {
                var customer = session.Get<Customer>(FirstCustomerId);
                return new CustomerDTO
                           {
                               FirstName = customer.FirstName,
                               LastName = customer.LastName,
                               Id = customer.Id
                           };
            }
        }
    }
}