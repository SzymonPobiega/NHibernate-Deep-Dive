using NHibernate;
using NHibernate_Deep_Dive.Entities;
using NUnit.Framework;

namespace NHibernate_Deep_Dive.Concurrency
{
    [TestFixture]
    public abstract class ConcurrencySpecification : SpecificationBase
    {
        [Test]
        public void Saving_stale_object_causes_exception()
        {
            using (var loserSession = OpenNamedSession(GetType().Name.Replace("Specification","")+"_Loser"))
            {
                var firstInstance = loserSession.Get<Customer>(CustomerId);

                using (var winnerSession = OpenNamedSession(GetType().Name.Replace("Specification","") + "_Winner"))
                using (var winnerTransaction = winnerSession.BeginTransaction())
                {
                    var secondInstance = winnerSession.Get<Customer>(CustomerId);
                    secondInstance.FirstName = "John";
                    winnerTransaction.Commit();
                }
                TestDelegate act = () =>
                                       {
                                           using (var loserTransaction = loserSession.BeginTransaction())
                                           {
                                               firstInstance.FirstName = "Jane";
                                               loserTransaction.Commit();
                                           }
                                       };
                Assert.Throws<StaleObjectStateException>(act);
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
    }
}