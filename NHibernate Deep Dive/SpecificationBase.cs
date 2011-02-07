using System;
using System.Collections.Generic;
using System.Data;
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

        protected virtual void BeforeTestRun()
        {
        }

        protected virtual void AfterTestRun()
        {
        }
        
        protected ISession OpenNamedSession(string name)
        {
            var session = SessionFactory.OpenSession();
            HibernatingRhinos.Profiler.Appender.NHibernate.NHibernateProfiler.RenameSessionInProfiler(session, name);
            return session;
        }

        protected ISession OpenNamedSession(string name, IDbConnection existingccConnection)
        {
            var session = SessionFactory.OpenSession(existingccConnection);
            HibernatingRhinos.Profiler.Appender.NHibernate.NHibernateProfiler.RenameSessionInProfiler(session, name);
            return session;
        }

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
            Configuration.SetDefaultAssembly("NHibernateDeepDive");
            Configuration.SetDefaultNamespace("NHibernate_Deep_Dive.Entities");
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

            BeforeTestRun();

            HibernatingRhinos.Profiler.Appender.NHibernate.NHibernateProfiler.Initialize();
        }

        [TearDown]
        public void TearDown()
        {
            HibernatingRhinos.Profiler.Appender.NHibernate.NHibernateProfiler.Stop();

            AfterTestRun();
        }
    }
}
