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
                                                           FirstName = "Szymon",
                                                           LastName = "Pobiega"
                                                       });
                SecondCustomerId = session.Save(new Customer
                                                    {
                                                        FirstName = "Michał",
                                                        LastName = "Wójcik"
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