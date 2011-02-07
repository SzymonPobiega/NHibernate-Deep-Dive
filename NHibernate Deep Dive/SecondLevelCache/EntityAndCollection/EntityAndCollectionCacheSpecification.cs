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
    public class EntityAndCollectionCacheSpecification : CacheSpecification
    {
        [Test]
        public void Whole_object_graph_is_stored_in_cache()
        {
            //Load from DB
            using (var session = OpenNamedSession("Entity and collection cache load"))
            {
                session.Get<Order>(OrderId);
            }

            //Then, load from cache
            using (var session = OpenNamedSession("Entity and collection cache hit"))
            {
                session.Get<Order>(OrderId);
            }
            
            SessionFactory.Statistics.SecondLevelCacheHitCount.Should().Be(4); //Order + Order.Categories + 2 x Category
        }

        protected override void AdjustConfiguration(NHibernate.Cfg.Configuration cfg)
        {
            //XmlConfigurator.Configure();
            cfg.Cache(x => x.Provider<HashtableCacheProvider>());
        }

        protected override string MappingsDirectory
        {
            get { return @"SecondLevelCache\EntityAndCollection"; }
        }
    }
}