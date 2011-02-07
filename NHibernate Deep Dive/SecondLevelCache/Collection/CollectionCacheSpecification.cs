using System.Configuration;
using System.Data.SqlClient;
using log4net.Config;
using NHibernate.Cache;
using NHibernate_Deep_Dive.Entities;
using NUnit.Framework;
using NHibernate.Cfg.Loquacious;
using FluentAssertions;

namespace NHibernate_Deep_Dive.SecondLevelCache.Collection
{
    [TestFixture]
    public class CollectionCacheSpecification : CacheSpecification
    {
        [Test]
        public void Only_child_collection_ids_are_stored_in_the_cache()
        {
            //Load from DB
            using (var session = OpenNamedSession("Collection cache load"))
            {
                session.Get<Order>(OrderId);
            }

            //Then, load from cache
            using (var session = OpenNamedSession("Collection cache hit"))
            {
                session.Get<Order>(OrderId);
            }
            
            SessionFactory.Statistics.SecondLevelCacheHitCount.Should().Be(1); //Collection cache hit
        }

        protected override void AdjustConfiguration(NHibernate.Cfg.Configuration cfg)
        {
            //XmlConfigurator.Configure();
            cfg.Cache(x => x.Provider<HashtableCacheProvider>());
        }

        protected override string MappingsDirectory
        {
            get { return @"SecondLevelCache\Collection"; }
        }
    }
}