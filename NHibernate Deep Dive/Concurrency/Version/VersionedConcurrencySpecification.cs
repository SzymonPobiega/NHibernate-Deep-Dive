using System;
using NUnit.Framework;

namespace NHibernate_Deep_Dive.Concurrency.Version
{
    [TestFixture]
    public class VersionedConcurrencySpecification : ConcurrencySpecification
    {
        protected override string MappingsDirectory
        {
            get { return @"Concurrency\Version"; }
        }
    }
}