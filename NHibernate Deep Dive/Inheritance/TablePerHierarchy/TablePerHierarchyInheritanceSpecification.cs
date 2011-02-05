using System.Collections;
using NUnit.Framework;

namespace NHibernate_Deep_Dive.Inheritance.TablePerHierarchy
{
    [TestFixture]
    public class TablePerHierarchyInheritanceSpecification : InheritanceSpecification
    {
        protected override string MappingsDirectory
        {
            get { return @"Inheritance\TablePerHierarchy"; }
        }
    }
}