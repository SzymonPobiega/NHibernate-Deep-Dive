using NUnit.Framework;

namespace NHibernate_Deep_Dive.Concurrency.Optimistic
{
    [TestFixture]
    public class OptimisticConcurrencySpecification : ConcurrencySpecification
    {
        protected override string MappingsDirectory
        {
            get { return @"Concurrency\Optimistic"; }
        }
    }
}