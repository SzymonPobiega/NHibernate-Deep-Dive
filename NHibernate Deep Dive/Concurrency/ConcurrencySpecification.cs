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
            using (var looserSession = SessionFactory.OpenSession())
            {
                var firstInstance = looserSession.Get<Customer>(CustomerId);
                
                using (var winnerSession = SessionFactory.OpenSession())
                using (var winnerTransaction = winnerSession.BeginTransaction())
                {
                    var secondInstance = winnerSession.Get<Customer>(CustomerId);
                    secondInstance.FirstName = "John";
                    winnerTransaction.Commit();
                }
                TestDelegate act = () =>
                                       {
                                           using (var looserTransaction = looserSession.BeginTransaction())
                                           {
                                               firstInstance.FirstName = "Jane";
                                               looserTransaction.Commit();
                                           }
                                       };
                Assert.Throws<StaleObjectStateException>(act);
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
    }
}