using NUnit.Framework;

namespace NHibernate_Deep_Dive.Concurrency.Timestamp
{
    [TestFixture]
    public class TimestampedConcurrencySpecification : ConcurrencySpecification
    {
        protected override string MappingsDirectory
        {
            get { return @"Concurrency\Timestamp"; }
        }
    }
}