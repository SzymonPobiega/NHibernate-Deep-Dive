using NHibernate_Deep_Dive.Entities;
using NUnit.Framework;

namespace NHibernate_Deep_Dive.Session
{
    public class SessionSpecification : SpecificationBase
    {
        protected object FirstCustomerId;
        protected object SecondCustomerId;

        [SetUp]
        public void PopulateDatabase()
        {
            using (var session = SessionFactory.OpenSession())
            using (var transaction = session.BeginTransaction())
            {
                FirstCustomerId = session.Save(new Customer
                                                       {
                                                           FirstName = "Terry",
                                                           LastName = "Gilliam"
                                                       });
                SecondCustomerId = session.Save(new Customer
                                                    {
                                                        FirstName = "Graham",
                                                        LastName = "Chapman"
                                                    });
                transaction.Commit();
            }
        }


        protected override string MappingsDirectory
        {
            get { return @"Session"; }
        }
    }
}