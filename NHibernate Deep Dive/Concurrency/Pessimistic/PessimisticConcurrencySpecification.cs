using System;
using NHibernate;
using NHibernate.Exceptions;
using NHibernate_Deep_Dive.Entities;
using NUnit.Framework;

namespace NHibernate_Deep_Dive.Concurrency.Pessimistic
{
    [TestFixture]
    public class PessimisticConcurrencySpecification : SpecificationBase
    {
        [Test]
        public void Saving_stale_object_causes_exception()
        {
            using (var winnerSession = SessionFactory.OpenSession())
            {
                using (var winnerTransaction = winnerSession.BeginTransaction())
                {
                    var firstInstance = winnerSession.Get<Customer>(CustomerId, LockMode.Upgrade);

                    TestDelegate act = () =>
                                           {
                                               using (var looserSession = SessionFactory.OpenSession())
                                               using (var looserTransaction = looserSession.BeginTransaction())
                                               {

                                                   var secondInstance = looserSession.Get<Customer>(CustomerId,
                                                                                                    LockMode.
                                                                                                        UpgradeNoWait);
                                                   secondInstance.FirstName = "John";
                                                   looserTransaction.Commit();
                                               }
                                           };
                    Assert.Throws<GenericADOException>(act);

                    firstInstance.FirstName = "Jane";
                    winnerTransaction.Commit();
                }
            }
        }

        protected object CustomerId;

        [SetUp]
        public void PopulateDatabase()
        {
            using (var session = SessionFactory.OpenSession())
            using (var transaction = session.BeginTransaction())
            {
                CustomerId = session.Save(new Customer
                {
                    FirstName = "Szymon",
                    LastName = "Pobiega"
                });
                transaction.Commit();
            }
        }

        protected override string MappingsDirectory
        {
            get { return @"Concurrency\Pessimistic"; }
        }
    }
}