using Knot.Business;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Knot.Tests
{
    [TestClass]
    public class FindTest
    {
        [ClassInitialize]
        public static void Populate(TestContext testContext)
        {
            AccessTest.PopulateSimple();
        }

        [TestMethod]
        public void TestNotFound()
        {
            var notfound = AccessTest.Instance.FindByName("notfound")
                .FirstOrDefault();

            Assert.IsNull(notfound);
        }

        [TestMethod]
        public void TestMathStartName()
        {
            var knots = AccessTest.Instance.FindByName(new Regex(@"^knot"))
                .ToArray();

            Assert.IsTrue(knots.Length > 2);
        }

        [TestMethod]
        public void FindFirst()
        {
            var knot2 = AccessTest.Instance.FindByName("knot2")
                .First();

            // test if not get childs
            Assert.IsNull(knot2.Childs);
            // test if not get parent
            Assert.IsNull(knot2.Parent);
            // test if load without props
            Assert.IsNull(knot2.Properties);
        }

        [TestMethod]
        public void FindById()
        {
            var knot2 = AccessTest.Instance.FindByName("knot2")
                .First();

            var knot2byId = AccessTest.Instance.FindById(knot2.IdKnot);

            Assert.IsTrue(knot2.IdKnot.Equals(knot2byId.IdKnot));
        }

        [TestMethod]
        public void FindWithProperties()
        {
            var knot2 = AccessTest.Instance.FindByName("knot2",
                new FindOptions { LoadProperties = true })
                .First();

            // test if load with props
            Assert.IsTrue(knot2.Properties.Count > 0);
        }

        [TestMethod]
        public void FindWithOneDeep()
        {
            var knot2 = AccessTest.Instance.FindByName("knot2",
                new FindOptions { ChildsDepth = 1, ParentDepth = 1 })
                .First();

            // test if get childs of 1 depth
            Assert.IsNotNull(knot2.Childs);
            // test if get parent of 1 depth
            Assert.IsNotNull(knot2.Parent);
            // test if load props for all
            Assert.IsNull(knot2.Properties);

            // test if the parent is right
            Assert.AreEqual(knot2.Parent.Name, "knot1");
            // test if not get parent of 2 depth
            Assert.IsNull(knot2.Parent.Parent);
            // test if not load parent props
            Assert.IsNull(knot2.Parent.Properties);


            foreach (var child in knot2.Childs)
            {
                // test if not get childs of 2 depth
                Assert.IsNull(child.Childs);
                // test if get childs of 1 depth has parent
                Assert.IsNotNull(child.Parent);
                // test if te childs start with knot3
                Assert.AreEqual(child.Name.Substring(0, 5), "knot3");
                // test if not load child props
                Assert.IsNull(child.Properties);
            }
        }

        [TestMethod]
        public void FindWithOneDeepWithProperties()
        {
            var knot2 = AccessTest.Instance.FindByName("knot2",
                new FindOptions { ChildsDepth = 1, ParentDepth = 1,
                    LoadChildsProperties = true, LoadParentProperties = true })
                    .First();

            // test if load without props
            Assert.IsNull(knot2.Properties);

            // test if load parent with props
            Assert.IsTrue(knot2.Parent.Properties.Count > 0);

            foreach (var child in knot2.Childs)
            {
                // test if load child with props
                Assert.IsNotNull(child.Properties);
            }
        }
    }
}
