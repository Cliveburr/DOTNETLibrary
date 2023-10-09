using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HackerRank
{
    class MagicSquareValue
    {
        public int Value { get; set; }
        public MagicSquareSum[] Affects { get; set; }
    }

    class MagicSquareSum
    {
        public MagicSquareValue[] Values { get; set; }
        public long Sum
        {
            get
            {
                return Values.Sum(v => v.Value);
            }
        }
    }

    public class MagicSquare
    {
        public static int formingMagicSquare(int[][] s)
        {
            // Complete this function
            var vs = s.Select(ss => ss.Select(sss => new MagicSquareValue { Value = sss }).ToArray()).ToArray();

            var d0 = new MagicSquareSum { Values = new MagicSquareValue[] { vs[0][0], vs[1][1], vs[2][2] } };
            var d1 = new MagicSquareSum { Values = new MagicSquareValue[] { vs[2][0], vs[1][1], vs[0][2] } };
            var sums = new List<MagicSquareSum> { d0, d1 };

            for (var i = 0; i < 3; i++)
            {
                sums.Add(new MagicSquareSum { Values = new MagicSquareValue[] { vs[i][0], vs[i][1], vs[i][2] } });
                sums.Add(new MagicSquareSum { Values = new MagicSquareValue[] { vs[0][i], vs[1][i], vs[2][i] } });
            }

            var vsrange = vs.SelectMany(vssa => vssa).ToArray();
            foreach (var value in vsrange)
            {
                value.Affects = sums.Where(sssums => sssums.Values.Contains(value)).ToArray();
            }

            //var target = sums
            //    .Select(sss => sss.Sum)
            //    .Distinct()
            //    .Select(sss => new { num = sss, appears = sums.Count(adf => adf.Sum == sss) })
            //    .OrderByDescending(adf => adf.appears)
            //    .First().num;

            var cost = 0;
            long target = 0;
            do
            {
                var groups = sums
                    .GroupBy(sss => sss.Sum)
                    .OrderByDescending(g => g.Count())
                    .ToList();

                var groupTarget = groups.First();
                target = groupTarget.Key;

                groups.Remove(groupTarget);

                if (groups.Count > 0)
                {
                    var tgroup = groups.First();

                    var bestValueArr = tgroup
                        .SelectMany(g => g.Values)
                        .GroupBy(g => g)
                        .OrderByDescending(g => g.Count())
                        .ToArray();

                    var bestValue = bestValueArr.First().Key;

                    var diff = target - tgroup.Key;

                    cost += (int)Math.Abs(diff);

                    bestValue.Value += (int)diff;
                }

                //var sums2 = sums.Where(sss => sss.Sum != target);

                //foreach (var sum in sums2)
                //{
                //    var bestValueArr = sum.Values
                //        .Select(v => new { value = v, above = v.Affects.Where(va => va.Sum < target).Count(), below = v.Affects.Where(va => va.Sum > target).Count() })
                //        .Where(v => v.above > 0 || v.below > 0)
                //        .ToArray();

                //    var bestValue = bestValueArr
                //        .Where(v => v.above > 0 ^ v.below > 0)
                //        .Select(v => new { value = v.value, affect = v.above + v.below })
                //        .OrderByDescending(v => v.affect)
                //        .First().value;

                //    var diff = target - sum.Sum;

                //    cost += (int)Math.Abs(diff);

                //    bestValue.Value += (int)diff;
                //}
            } while (sums.Where(sss => sss.Sum != target).Count() > 0);

            return cost;
        }
    }
}