using System;
using System.Configuration;
using System.Data.SqlClient;
using log4net.Config;
using NHibernate.Cache;
using NHibernate_Deep_Dive.Entities;
using NUnit.Framework;
using NHibernate.Cfg.Loquacious;
using FluentAssertions;

namespace NHibernate_Deep_Dive.SecondLevelCache.Entity
{
    [TestFixture]
    public class EntityCacheSpecification : CacheSpecification
    {
        [Test]
        public void Cached_entity_will_be_returned_from_cache_when_searched_by_id()
        {
            //First, load from DB
            using (var session = OpenNamedSession("Entity cache load"))
            using (var transaction = session.BeginTransaction())
            {
                var category = session.Get<Category>(FirstCategoryId);
                category.Name = "Some other name";
                transaction.Commit();
            }
            SessionFactory.Statistics.SecondLevelCacheHitCount.Should().Be(0);

            //Then, load from cache
            using (var session = OpenNamedSession("Entity cache hit"))
            {
                var category = session.Get<Category>(FirstCategoryId);
                category.Name.Should().Be("Some other name");
            }
            SessionFactory.Statistics.SecondLevelCacheHitCount.Should().Be(1);
        }

        [Test]
        public void Second_level_cache_does_not_work_without_explicit_transactions()
        {
            //First, load from DB
            using (var session = OpenNamedSession("Entity cache load and object modification"))
            {
                var category = session.Get<Category>(FirstCategoryId);
                category.Name = "Some other name";
                session.Flush();
            }
            SessionFactory.Statistics.SecondLevelCacheHitCount.Should().Be(0);

            //Load from DB again. Cache doesn't work!
            using (var session = OpenNamedSession("Entity cache miss"))
            {
                session.Get<Category>(FirstCategoryId);
            }
            SessionFactory.Statistics.SecondLevelCacheHitCount.Should().Be(0);
        }

        [Test]
        public void Second_level_cache_does_not_work_with_user_provided_connections()
        {
            var connectionString = ConfigurationManager.ConnectionStrings["db"].ConnectionString;

            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                using (var session = OpenNamedSession("Entity cache load", connection))
                {
                    session.Get<Category>(FirstCategoryId);
                }
            }

            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                using (var session = OpenNamedSession("Entity cache miss", connection))
                {
                    session.Get<Category>(FirstCategoryId);
                }
            }
            //Cache won't be used
            SessionFactory.Statistics.SecondLevelCacheHitCount.Should().Be(0);
        }

        protected override void AdjustConfiguration(NHibernate.Cfg.Configuration cfg)
        {
            //XmlConfigurator.Configure();
            cfg.Cache(x => x.Provider<HashtableCacheProvider>());
        }

        protected override string MappingsDirectory
        {
            get { return @"SecondLevelCache\Entity"; }
        }
    }
}