using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using NHibernate;
using NHibernate.ByteCode.Castle;
using NHibernate.Cfg;
using NHibernate.Cfg.Loquacious;
using NHibernate.Context;
using NHibernate.Dialect;
using NHibernate.Tool.hbm2ddl;
using NUnit.Framework;

namespace NHibernate_Deep_Dive
{
    public abstract class SpecificationBase
    {
        protected abstract string MappingsDirectory { get; }
        protected virtual void AdjustConfiguration(Configuration cfg)
        {
        }

        protected Configuration Configuration { get; private set; }
        protected ISessionFactory SessionFactory { get; private set; }

        [SetUp]
        public void PrepareSessionFactory()
        {
            Configuration = new Configuration();
            Configuration.Proxy(p => p.ProxyFactoryFactory<ProxyFactoryFactory>())
                .DataBaseIntegration(db =>
                                         {
                                             db.ConnectionStringName = "db";
                                             db.Dialect<MsSql2008Dialect>();
                                         });
            Configuration.SetProperty("show_sql", "true");
            Configuration.SetDefaultAssembly("NHibernate Customer Mapping");
            Configuration.SetDefaultNamespace("NHibernate_Customer_Mapping.Entities");
            Configuration.AddXmlFile("ClearDatabaseScript.hbm.xml");
            foreach (var mappingFile in Directory.GetFiles(MappingsDirectory))
            {
                Configuration.AddXmlFile(mappingFile);
            }
            AdjustConfiguration(Configuration);
            Configuration.SessionFactory().GenerateStatistics();
            
            SessionFactory = Configuration.BuildSessionFactory();

            //new SchemaExport(Configuration).Drop(false, true);
            new SchemaExport(Configuration).Execute(false, true, false);
        }
    }
}
