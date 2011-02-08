using FluentAssertions;
using NHibernate.Cfg.Loquacious;
using NHibernate.Context;
using NHibernate_Deep_Dive.Entities;
using NUnit.Framework;

namespace NHibernate_Deep_Dive.Session.Management
{
    public class AmbientSessionManagementSpecification : SessionSpecification
    {
        [Test]
        public void Ambient_session_can_be_accessed_from_anywhere_in_the_code()
        {
            using (var session = SessionFactory.OpenSession())
            {                
                //This goes to the infrastructure (WCF Message Inspector, ASP.NET Http Module)
                CurrentSessionContext.Bind(session);
                HandleWebRequest();
                CurrentSessionContext.Unbind(SessionFactory);
            }

            //Validate
            using (var session = SessionFactory.OpenSession())
            {
                var customer = session.Get<Customer>(FirstCustomerId);
                customer.FirstName.Should().Be("John");
            }
        }

        private void HandleWebRequest()
        {
            var customer = GetCustomerForDisplay();
            UpdateCustomer(customer);
        }

        private void UpdateCustomer(Customer customer)
        {
            var session = SessionFactory.GetCurrentSession();
            using (var transaction = session.BeginTransaction())
            {
                customer.FirstName = "John";
                transaction.Commit();                
            }
        }

        private Customer GetCustomerForDisplay()
        {
            var session = SessionFactory.GetCurrentSession();
            return session.Get<Customer>(FirstCustomerId);
        }

        protected override void AdjustConfiguration(NHibernate.Cfg.Configuration cfg)
        {
            /*
             * NHibernate.Context.CallSessionContext
             * NHibernate.Context.ManagedWebSessionContext
             * NHibernate.Context.MapBasedSessionContext
             * NHibernate.Context.ReflectiveHttpContext
             * NHibernate.Context.ThreadLocalSessionContext
             * NHibernate.Context.ThreadStaticSessionContext
             * NHibernate.Context.WcfOperationSessionContext
             * NHibernate.Context.WebSessionContext
             */
            cfg.CurrentSessionContext<ThreadStaticSessionContext>();
        }
    }
}