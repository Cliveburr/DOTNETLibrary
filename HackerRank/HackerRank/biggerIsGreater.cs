using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HackerRank
{
    class biggerIsGreaterNum
    {
        public int Index { get; set; }
        public byte Value { get; set; }
        public bool Swaped { get; set; }
    }

    public static class biggerIsGreater
    {
        static void biggerIsGreaterSwap(biggerIsGreaterNum[] w, int a, int b)
        {
            var temp = w[a];
            w[a] = w[b];
            w[b] = temp;

            w[a].Swaped = true;
            w[b].Swaped = true;
        }

        public static string run(string w)
        {
            // Complete this function greater / after

            var i = 0;
            var codes = System.Text.Encoding.ASCII.GetBytes(w)
                .Select(n => new { index = i++, value = n })
                .ToArray();

            var first = codes.First();

            var possibleSmallest = codes
                .Where(c => c.value > first.value)
                .OrderByDescending(c => c.index)
                .FirstOrDefault();
            //.ToArray();

            //var tests = new List<string>();
            //foreach (var possible in possibleSmallest)
            //{
            var i2 = 0;
            var t = System.Text.Encoding.ASCII.GetBytes(w)
                .Select(n => new biggerIsGreaterNum { Index = i2++, Value = n, Swaped = false })
                .ToArray();

            if (possibleSmallest != null)
                biggerIsGreaterSwap(t, 0, possibleSmallest.index);

            biggerIsGreaterNum smaller = null;
            biggerIsGreaterNum highest = null;
            do
            {
                smaller = t
                    .Where(c => !c.Swaped)
                    .OrderBy(c => c.Value)
                    .FirstOrDefault();

                if (smaller != null)
                {
                    highest = t
                        .Where(c => !c.Swaped)
                        .OrderByDescending(c => c.Value)
                        .FirstOrDefault();

                    if (highest != null)
                    {
                        biggerIsGreaterSwap(t, smaller.Index, highest.Index);
                    }
                }

            } while (smaller != null && highest != null);

            var strBytes = t
                    .Select(c => c.Value)
                    .ToArray();

            var tr = System.Text.Encoding.ASCII.GetString(strBytes);

            //    tests.Add(str);
            //}

            //var tr = tests
            //    .OrderBy(t => t)
            //    .FirstOrDefault();

            return tr == null || tr == w ?
                "no answer" :
                tr;
        }
    }
}