using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using FluentAssertions;
using NHibernate;
using NHibernate.Cfg.Loquacious;
using NHibernate.Context;
using NHibernate_Deep_Dive.Entities;
using NUnit.Framework;

namespace NHibernate_Deep_Dive.Session.Management
{
    public class DurableSessionManagementSpecification : SessionSpecification
    {
        private byte[] _sessionStorage;

        [Test]
        public void Session_can_be_serialized_and_stored_between_requests()
        {
            HandleFirstWebRequest();
            HandleSecondWebRequest();

            //Validate
            using (var session = SessionFactory.OpenSession())
            {
                var customer = session.Get<Customer>(FirstCustomerId);
                customer.FirstName.Should().Be("John");
                customer.LastName.Should().Be("Doe");
            }
        }

        private void HandleSecondWebRequest()
        {
            var session = DeseializeSession(_sessionStorage);
            using (var transaction = session.BeginTransaction())
            {
                var customer = session.Get<Customer>(FirstCustomerId);
                customer.LastName = "Doe";
                transaction.Commit();
            }
        }

        private void HandleFirstWebRequest()
        {
            var session = SessionFactory.OpenSession();
            var customer = session.Get<Customer>(FirstCustomerId);
            customer.FirstName = "John";
            _sessionStorage = SeializeSession(session);
        }

        private static byte[] SeializeSession(ISession session)
        {
            var bf = new BinaryFormatter();
            var memory = new MemoryStream();
            bf.Serialize(memory, session);
            memory.Flush();
            return memory.GetBuffer();
        }

        private static ISession DeseializeSession(byte[] serializedSession)
        {
            var bf = new BinaryFormatter();
            var memory = new MemoryStream(serializedSession);
            return (ISession)bf.Deserialize(memory);
        }
    }
}