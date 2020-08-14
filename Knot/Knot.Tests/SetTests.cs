using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Knot.Tests
{
    [TestClass]
    public class SetTests
    {
        [TestMethod]
        public void TryInsertWithNoParent()
        {
            try
            {
                var dont = AccessTest.Instance.Set(new Entities.Knot
                {
                    Name = "dont"
                });
            }
            catch (Exception err)
            {
                Assert.IsTrue(err.Message.Contains("Invalid parent"));
                return;
            }
            Assert.Fail("Insert with no parent accept!");
        }

        [TestMethod]
        public void InsertAndUpdateWithOneChild()
        {
            var set0 = AccessTest.Instance.Set(new Entities.Knot
            {
                Name = "set0",
                Parent = AccessTest.Instance.GetRootKnot()
            });
            set0.Childs = new List<Entities.Knot>
            {
                new Entities.Knot
                {
                    Name = "set0child0"
                }
            };
            AccessTest.Instance.Set(set0);

            var set0child0 = AccessTest.Instance.FindByName("set0child0")
                .First();

            Assert.IsTrue(set0child0.IdParent.Equals(set0.IdKnot));
        }

        [TestMethod]
        public void InsertMultiplesChildLevel()
        {
            AccessTest.Instance.Set(new Entities.Knot
            {
                Name = "set1",
                Parent = AccessTest.Instance.GetRootKnot(),
                Childs = new List<Entities.Knot>
                {
                    new Entities.Knot
                    {
                        Name = "set1child0",
                        Childs = new List<Entities.Knot>
                        {
                            new Entities.Knot
                            {
                                Name = "set1child0sub0"
                            }
                        }
                    },
                    new Entities.Knot
                    {
                        Name = "set1child1",
                        Childs = new List<Entities.Knot>
                        {
                            new Entities.Knot
                            {
                                Name = "set1child1sub0"
                            }
                        }
                    }
                }
            });

            var sets = AccessTest.Instance.FindByName(new Regex("^set"))
                .ToArray();

            Assert.IsTrue(sets.Length == 5);
        }

        [TestMethod]
        public void ExcludeChildNotReferencied()
        {
            AccessTest.Instance.Set(new Entities.Knot
            {
                Name = "set2",
                Parent = AccessTest.Instance.GetRootKnot(),
                Childs = new List<Entities.Knot>
                {
                    new Entities.Knot
                    {
                        Name = "set2child0",
                        Childs = new List<Entities.Knot>
                        {
                            new Entities.Knot
                            {
                                Name = "set2child0sub0"
                            }
                        }
                    }
                }
            });

            var set2try = AccessTest.Instance.FindByName("set2")
                .First();
            set2try.Childs = new List<Entities.Knot>();

            try
            {
                AccessTest.Instance.Set(set2try);
            }
            catch (Exception err)
            {
                Assert.IsTrue(err.Message.Contains("Can only set childs"));
            }

            var set2 = AccessTest.Instance.FindByName("set2",
                new Business.FindOptions { ChildsDepth = 0 })
                .First();
            set2.Childs = new List<Entities.Knot>();
            AccessTest.Instance.Set(set2);

            var set2child0 = AccessTest.Instance.FindByName("set2child0")
                .FirstOrDefault();
            Assert.IsNull(set2child0);

            var set2child0sub0 = AccessTest.Instance.FindByName("set2child0sub0")
                .FirstOrDefault();
            Assert.IsNull(set2child0sub0);
        }
    }
}