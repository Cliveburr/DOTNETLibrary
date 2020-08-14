using Knot.Business;
using System;
using System.Collections.Generic;
using System.Text;

namespace Knot.Tests
{
    public static class AccessTest
    {
        private static KnotAccess _access;

        public static KnotAccess Instance
        {
            get
            {
                if (_access == null)
                {
                    var connectionString = "mongodb://localhost:27017";
                    var database = "Knot";
                    var knotsCollection = "KnotTests";

                    _access = new KnotAccess(connectionString, database, knotsCollection);

                    _access.Database.DropCollection(knotsCollection);
                }
                return _access;
            }
        }

        public static void PopulateSimple()
        {
            var knot0 = Instance.Set(new Entities.Knot
            {
                Name = "knot0",
                Parent = Instance.GetRootKnot()
            });
            knot0.Properties.Add("0prop0", "value zero");
            Instance.Set(knot0);

            var knot1 = Instance.Set(new Entities.Knot
            {
                Name = "knot1",
                Parent = knot0
            });
            knot1.Properties.Add("1prop0", "value um");
            Instance.Set(knot1);

            var knot2 = Instance.Set(new Entities.Knot
            {
                Name = "knot2",
                Parent = knot1
            });
            knot2.Properties.Add("2prop0", "value two");
            Instance.Set(knot2);

            var knot22 = Instance.Set(new Entities.Knot
            {
                Name = "knot22",
                Parent = knot1
            });
            knot22.Properties.Add("22prop0", 33);
            Instance.Set(knot22);

            var knot3 = Instance.Set(new Entities.Knot
            {
                Name = "knot3",
                Parent = knot2
            });
            knot3.Properties.Add("3prop0", 44);
            Instance.Set(knot3);

            var knot33 = Instance.Set(new Entities.Knot
            {
                Name = "knot33",
                Parent = knot2
            });

            var knot4 = Instance.Set(new Entities.Knot
            {
                Name = "knot4",
                Parent = knot3
            });
        }
    }
}