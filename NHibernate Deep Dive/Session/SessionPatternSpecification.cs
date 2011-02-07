using FluentAssertions;
using NHibernate_Deep_Dive.Entities;
using NUnit.Framework;

namespace NHibernate_Deep_Dive.Session
{
    public class SessionPatternSpecification : SessionSpecification
    {
        [Test]
        public void Session_implements_Identity_Map()
        {
            using (var session = OpenNamedSession("Session_implements_Identity_Map"))
            {
                var firstInstance = session.Get<Customer>(FirstCustomerId);

                var secondInstance = session.Get<Customer>(FirstCustomerId);

                firstInstance.Should().BeSameAs(secondInstance); //object.ReferenceEquals
            }
        }

        [Test]
        public void Session_can_be_used_as_Unit_of_Work()
        {
            using (var session = OpenNamedSession("Session_can_be_used_as_Unit_of_Work"))
            {
                var customer = session.Get<Customer>(FirstCustomerId);
                customer.FirstName = "Graham";

                session.Flush(); //Forces synchronizing changes to DB
            }

            using (var session = OpenNamedSession("Check results"))
            {
                var customer = session.Get<Customer>(FirstCustomerId);
                customer.FirstName.Should().Be("Graham");
            }
        }

        [Test]
        public void Session_and_Transactions_form_better_Unit_of_Work()
        {
            using (var session = OpenNamedSession("Session_and_Transactions_form_better_Unit_of_Work"))
            using (var transaction = session.BeginTransaction())
            {
                var customer = session.Get<Customer>(FirstCustomerId);
                customer.FirstName = "Graham";

                transaction.Commit(); //Forces synchronizing changes to DB
            }

            using (var session = OpenNamedSession("Check results"))
            {
                var customer = session.Get<Customer>(FirstCustomerId);
                customer.FirstName.Should().Be("Graham");
            }
        }
    }
}