using System;
using FluentAssertions;
using NHibernate;
using NHibernate_Deep_Dive.Entities;
using NUnit.Framework;

namespace NHibernate_Deep_Dive.Session.Management
{
    public class PerFormSessionManagementUsingUpdateSpecification : SessionSpecification
    {
        [Test]
        public void Using_per_form_session_with_Update_to_synchronize_model_changes()
        {
            using (var form = new Form(FirstCustomerId, SessionFactory))
            {
                form.Model.FirstName = "John";  //poor-man's data binding
                form.Save();
            }

            //Validate
            using (var session = SessionFactory.OpenSession())
            {
                var customer = session.Get<Customer>(FirstCustomerId);
                customer.FirstName.Should().Be("John");
            }
        }

        public class Form : IDisposable
        {
            private ISession _session;
            private readonly ISessionFactory _sessionFactory;
            private Customer _model;

            public Form(object customerId, ISessionFactory sessionFactory)
            {
                _sessionFactory = sessionFactory;
                _session = _sessionFactory.OpenSession();
                _model = _session.Get<Customer>(customerId);
            }

            public Customer Model
            {
                get { return _model; }
            }

            public void Save()
            {
                using (var transaction = _session.BeginTransaction())
                {
                    _session.Update(Model);
                    transaction.Commit();
                }
            }

            public void Dispose()
            {
                _session.Dispose();
            }
        }
    }
}