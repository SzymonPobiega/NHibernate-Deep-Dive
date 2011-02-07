using FluentAssertions;
using NHibernate_Deep_Dive.Entities;
using NUnit.Framework;

namespace NHibernate_Deep_Dive.Relations.ManyToManyBidirectional
{
    [TestFixture]
    public class ManyToManyRelationBidirectional: SpecificationBase
    {
        [Test]
        public void AddCategoryToOrder()
        {
            using (var session = SessionFactory.OpenSession())
            using (var transaction = session.BeginTransaction())
            {
                Order order = session.Get<Order>(OrderId);
                Category category = session.Get<Category>(CategoryId);

                order.Categories.Add(category);

                session.Save(order);
                transaction.Commit();
            }

            using (var session = SessionFactory.OpenSession())
            {
                Order order = session.Get<Order>(OrderId);
                Category category = session.Get<Category>(CategoryId);

                order.Categories.Should().Contain(category);
                category.Orders.Should().Contain(order);
            }
        }

        [Test]
        public void AddOrderToCategory()
        {
            using (var session = SessionFactory.OpenSession())
            using (var transaction = session.BeginTransaction())
            {
                Order order = session.Get<Order>(OrderId);
                Category category = session.Get<Category>(CategoryId);

                category.Orders.Add(order);

                session.Save(category);
                transaction.Commit();
            }

            using (var session = SessionFactory.OpenSession())
            {
                Order order = session.Get<Order>(OrderId);
                Category category = session.Get<Category>(CategoryId);

                order.Categories.Should().Contain(category);
                category.Orders.Should().Contain(order);
            }
        }

        protected override string MappingsDirectory
        {
            get { return @"Relations\ManyToManyBidirectional"; }
        }

        protected object CategoryId;
        protected object OrderId;


        protected override void PopulateDatabase()
        {
            using (var session = SessionFactory.OpenSession())
            using (var transaction = session.BeginTransaction())
            {
                OrderId = session.Save(new Order()
                {
                    Value = 3
                });

                CategoryId = session.Save(new Category() {Name = "Important"});
                transaction.Commit();
            }
        }
    }
}