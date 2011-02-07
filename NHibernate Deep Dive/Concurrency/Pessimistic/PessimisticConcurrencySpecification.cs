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
            using (var winnerSession = OpenNamedSession("PessimisticConcurrency_Winner"))
            {
                using (var winnerTransaction = winnerSession.BeginTransaction())
                {
                    var firstInstance = winnerSession.Get<Customer>(CustomerId, LockMode.Upgrade);

                    TestDelegate act = () =>
                                           {
                                               using (var loserSession = OpenNamedSession("PessimisticConcurrency_Loser"))
                                               using (var loserTransaction = loserSession.BeginTransaction())
                                               {

                                                   var secondInstance = loserSession.Get<Customer>(CustomerId,
                                                                                                    LockMode.
                                                                                                        UpgradeNoWait);
                                                   secondInstance.FirstName = "John";
                                                   loserTransaction.Commit();
                                               }
                                           };
                    Assert.Throws<GenericADOException>(act);

                    firstInstance.FirstName = "Jane";
                    winnerTransaction.Commit();
                }
            }
        }

        protected object CustomerId;

        protected override void BeforeTestRun()
        {
            using (var session = SessionFactory.OpenSession())
            using (var transaction = session.BeginTransaction())
            {
                CustomerId = session.Save(new Customer
                {
                    FirstName = "Terry",
                    LastName = "Gilliam"
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