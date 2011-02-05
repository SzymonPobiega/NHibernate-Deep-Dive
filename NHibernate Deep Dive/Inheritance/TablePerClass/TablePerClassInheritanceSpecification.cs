using NUnit.Framework;

namespace NHibernate_Deep_Dive.Inheritance.TablePerClass
{
    [TestFixture]
    public class TablePerClassInheritanceSpecification : InheritanceSpecification
    {
        protected override string MappingsDirectory
        {
            get { return @"Inheritance\TablePerClass"; }
        }
    }
}