using NUnit.Framework;

namespace NHibernate_Deep_Dive.Inheritance.TablePerConcreteClass
{
    [TestFixture]
    public class TablePerConcreteClassInheritanceSpecification : InheritanceSpecification
    {
        protected override string MappingsDirectory
        {
            get { return @"Inheritance\TablePerConcreteClass"; }
        }
    }
}