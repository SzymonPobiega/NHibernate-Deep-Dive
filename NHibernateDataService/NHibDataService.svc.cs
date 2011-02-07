using System;
using System.Collections.Generic;
using System.Data.Services;
using System.Data.Services.Common;
using System.IO;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Web;
using NHibernate;
using NHibernate.ByteCode.Castle;
using NHibernate.Cache;
using NHibernate.Cfg;
using NHibernate.Cfg.Loquacious;
using NHibernate.Dialect;
using NHibernate.Linq;
using NHibernate.Tool.hbm2ddl;
using Customer = NHibernateDataService.Entities.Customer;
using Order = NHibernateDataService.Entities.Order;

namespace NHibernateDataService
{
    [ServiceBehavior(IncludeExceptionDetailInFaults = true)]
    public class NHibDataService : DataService<MyNHibernateDataContext>
    {
        // This method is called only once to initialize service-wide policies.
        public static void InitializeService(DataServiceConfiguration config)
        {
            // TODO: set rules to indicate which entity sets and service operations are visible, updatable, etc.
            // Examples:
            config.SetEntitySetAccessRule("*", EntitySetRights.AllRead);
            // config.SetServiceOperationAccessRule("MyServiceOperation", ServiceOperationRights.All);
            config.DataServiceBehavior.MaxProtocolVersion = DataServiceProtocolVersion.V2;
        }

        ISession session;
        protected override MyNHibernateDataContext CreateDataSource()
        {
            var factory = CreateSessionFactory();
            this.session = factory.OpenSession();
            this.session.FlushMode = FlushMode.Auto;
            PrepareDatabase(factory);
            return new MyNHibernateDataContext(this.session);
        }

        private static ISessionFactory CreateSessionFactory()
        {
            Configuration Configuration = new Configuration();
            Configuration.Proxy(p => p.ProxyFactoryFactory<ProxyFactoryFactory>())
                .DataBaseIntegration(db =>
                {
                    db.ConnectionStringName = "db";
                    db.Dialect<MsSql2008Dialect>();
                });
            Configuration.SetProperty("show_sql", "true");
            Configuration.SetDefaultAssembly("NHibernateDataService");
            Configuration.SetDefaultNamespace("NHibernateDataService.Entities");
            
            Configuration.AddAssembly("NHibernateDataService");

            
            Configuration.SessionFactory().GenerateStatistics();

            ISessionFactory SessionFactory = Configuration.BuildSessionFactory();

            //new SchemaExport(Configuration).Drop(false, true);
            new SchemaExport(Configuration).Execute(false, true, false);

            return SessionFactory;
        }

        private void PrepareDatabase(ISessionFactory SessionFactory)
        {
            using (ISession session = SessionFactory.OpenSession())
            using (ITransaction transaction = session.BeginTransaction())
            {
                for (int i = 0; i < 10; i++)
                {
                    Customer customer = new Customer
                    {
                        FirstName = "Terry" + i.ToString(),
                        LastName = "Gilliam" + i.ToString()
                    };
                    for (int j = 10 - i; j > 0; j--)
                    {
                        customer.Orders.Add(new Order() { Value = j, Customer = customer });
                    }

                    session.Save(customer);
                }

                transaction.Commit();
            }
        }
    }

    public class MyNHibernateDataContext
    {
        private ISession _session;
        public MyNHibernateDataContext(ISession session)
        {
            _session = session;
        }    
        
        public IQueryable<Customer> Customers
        {
            get { return new NhQueryable<Customer>(_session); }
        }

        public IQueryable<Order> Orders
        {
            get { return new NhQueryable<Order>(_session); }
        }   
    }

}