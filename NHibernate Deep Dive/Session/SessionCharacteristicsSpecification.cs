using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using FluentAssertions;
using NHibernate;
using NHibernate_Deep_Dive.Entities;
using NUnit.Framework;

namespace NHibernate_Deep_Dive.Session
{
    public class SessionCharacteristicsSpecification : SessionSpecification
    {
        [Test]
        public void Session_can_be_serialized_between_multiple_requests()
        {
            var bf = new BinaryFormatter();

            var session = SessionFactory.OpenSession();

            var firstInstance = session.Get<Customer>(FirstCustomerId);
            firstInstance.Should().NotBeNull();

            var memory = new MemoryStream();
            bf.Serialize(memory, session);
            memory.Flush();
            memory.Seek(0, SeekOrigin.Begin);

            session = (ISession)bf.Deserialize(memory);

            var secondInstance = session.Get<Customer>(SecondCustomerId);
            secondInstance.Should().NotBeNull();

            session.Dispose();
        }

        [Test]
        public void Changes_to_objects_are_preserved_when_serializng_session()
        {
            var session = SessionFactory.OpenSession();

            var firstInstance = session.Get<Customer>(FirstCustomerId);
            firstInstance.FirstName = "John";
            firstInstance.Should().NotBeNull();

            session = SerializeAndDeserializeSession(session);

            var secondInstance = session.Get<Customer>(FirstCustomerId);
            secondInstance.FirstName.Should().Be("John");

            session.Dispose();
        }

        [Test]
        public void Session_creation_is_VERY_CHEAP()
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();
            for (int i = 0; i < 1000; i++)
            {
                using (var session = SessionFactory.OpenSession())
                {
                    session.IsConnected.Should().BeTrue();
                }
            }
            sw.Stop();
            Console.WriteLine("Elapsed milliseconds: {0}", sw.ElapsedMilliseconds);
        }

        [Test]
        public void Object_can_be_moved_from_one_session_to_another()
        {
            Customer customer;
            using (var session = SessionFactory.OpenSession())
            {
                customer = session.Get<Customer>(FirstCustomerId);
                customer.FirstName = "John";
                session.Evict(customer);
            }

            using (var session = SessionFactory.OpenSession())
            {
                session.IsDirty().Should().BeFalse();

                session.Update(customer);

                session.IsDirty().Should().BeTrue();
                session.Flush();
            }
            using (var session = SessionFactory.OpenSession())
            {
                customer = session.Get<Customer>(FirstCustomerId);
                customer.FirstName.Should().Be("John");
            }
        }

        private static ISession SerializeAndDeserializeSession(ISession session)
        {
            var bf = new BinaryFormatter();

            var memory = new MemoryStream();
            bf.Serialize(memory, session);
            memory.Flush();
            memory.Seek(0, SeekOrigin.Begin);

            session = (ISession)bf.Deserialize(memory);
            return session;
        }
    }
}