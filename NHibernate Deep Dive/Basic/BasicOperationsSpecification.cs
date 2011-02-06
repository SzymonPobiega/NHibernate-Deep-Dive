using System;
using NHibernate_Deep_Dive.Entities;
using NUnit.Framework;

namespace NHibernate_Deep_Dive.Basic
{
    [TestFixture]
    public class BasicOperationsSpecification : SpecificationBase
    {
        private object _customerId;

        [Test]
        public void NHibernate_allows_crud_operations()
        {
            Create();
            Read();
            Update();
            Delete();
        }

        private void Create()
        {
            using (var session = SessionFactory.OpenSession())
            {
                var customer = new Customer()
                                   {
                                       FirstName = "John",
                                       LastName = "Cleese"
                                   };
                _customerId = session.Save(customer);
                session.Flush();
            }
        }

        private void Read()
        {
            using (var session = SessionFactory.OpenSession())
            {
                var customer = session.Get<Customer>(_customerId);
                Console.WriteLine("Hello, {0}!", customer.FirstName);
            }
        }

        private void Update()
        {
            using (var session = SessionFactory.OpenSession())
            {
                var customer = session.Get<Customer>(_customerId);
                customer.FirstName = "Graham";
                customer.LastName = "Chapman";
                session.Flush();
            }
        }

        private void Delete()
        {
            using (var session = SessionFactory.OpenSession())
            {
                var customer = session.Get<Customer>(_customerId);
                session.Delete(customer);
                session.Flush();
            }
        }

        protected override string MappingsDirectory
        {
            get { return @"Basic"; }
        }
    }
}