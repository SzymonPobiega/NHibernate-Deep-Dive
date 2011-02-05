using log4net.Config;
using NHibernate;
using NHibernate.Cache;
using NHibernate_Deep_Dive.Entities;
using NUnit.Framework;
using NHibernate.Cfg.Loquacious;
using FluentAssertions;

namespace NHibernate_Deep_Dive.SecondLevelCache.Query
{
    [TestFixture]
    public class QueryCacheSpecification : CacheSpecification
    {
        [Test]
        public void Only_query_result_ids_are_stored_in_query_cache()
        {
            //Load from DB
            using (var session = SessionFactory.OpenSession())
            {
                session.CreateCriteria<Category>()
                    .SetCacheable(true)
                    .List<Category>();
            }

            //Then, load from cache
            using (var session = SessionFactory.OpenSession())
            {
                session.CreateCriteria<Category>()
                   .SetCacheable(true)
                   .List<Category>();
            }
            
            SessionFactory.Statistics.QueryCacheHitCount.Should().Be(1);
        }

        protected override void AdjustConfiguration(NHibernate.Cfg.Configuration cfg)
        {
            //XmlConfigurator.Configure();
            cfg.Cache(x =>
                          {
                              x.UseQueryCache = true;
                              x.Provider<HashtableCacheProvider>();
                          });
        }

        protected override string MappingsDirectory
        {
            get { return @"SecondLevelCache\Query"; }
        }
    }
}