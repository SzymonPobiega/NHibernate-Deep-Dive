using NUnit.Framework;

namespace NHibernate_Deep_Dive.Concurrency.DbVersion
{
    [TestFixture]
    public class DbVersionedConcurrencySpecification : ConcurrencySpecification
    {
        protected override string MappingsDirectory
        {
            get { return @"Concurrency\DbVersion"; }
        }
    }
}