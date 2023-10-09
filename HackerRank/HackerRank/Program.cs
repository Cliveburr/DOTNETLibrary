using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HackerRank
{
    class Program
    {
        static int ReadInt()
        {
            return Convert.ToInt32(Console.ReadLine());
        }

        static int[] ReadInts()
        {
            return Array.ConvertAll(Console.ReadLine().Split(' '), Int32.Parse);
        }

        static void Main(string[] args)
        {
            Console.WriteLine(fairRations(new int[] { 1, 2 }));
            Console.WriteLine(fairRations(new int[] { 2, 3, 4, 5, 6 }));

            //Console.WriteLine(workbook(5, 3, new int[] { 4, 2, 6, 1, 10 }));

            //Console.WriteLine(chocolateFeast(10, 2, 5));
            //Console.WriteLine(chocolateFeast(12, 4, 4));
            //Console.WriteLine(chocolateFeast(6, 2, 2));

            //Console.WriteLine(string.Join(", ", kaprekarNumbers(400, 700)));
            //Console.WriteLine(string.Join(", ", kaprekarNumbers(1, 99999)));

            //Console.WriteLine(saveThePrisoner(5, 2, 2));
            //Console.WriteLine(saveThePrisoner(2, 576581, 1));

            //Console.WriteLine(viralAdvertising(4));

            //Console.WriteLine(flatlandSpaceStations(6, new int[] { 0, 1, 2, 4, 3, 5 }));
            //Console.WriteLine(flatlandSpaceStations(95, new int[] { 68, 81, 46, 54, 30, 11, 19, 23, 22, 12, 38, 91, 48, 75, 26, 86, 29, 83, 62 }));

            //Console.WriteLine(beautifulTriplets(3, new int[] { 1, 2, 4, 5, 7, 8, 10 }));

            //Console.WriteLine(acmTeam(new string[] { "10101", "11100", "11010", "00101" }));

            //Console.WriteLine(taumBday(56399, 55940, 594189, 471473, 231368));
            //Console.WriteLine(taumBday(71805, 9169, 905480, 255669, 334440));

            //Console.WriteLine(taumBday(10, 10, 1, 1, 1));
            //Console.WriteLine(taumBday(5, 9, 2, 3, 4));
            //Console.WriteLine(taumBday(3, 6, 9, 1, 1));
            //Console.WriteLine(taumBday(7, 7, 4, 2, 1));
            //Console.WriteLine(taumBday(3, 3, 1, 9, 2));

            //Console.WriteLine(twoArrays(10, new int[] { 2, 1, 3 }, new int[] { 7, 8, 9 }));
            //Console.WriteLine(twoArrays(5, new int[] { 1, 2, 2, 1 }, new int[] { 3, 3, 3, 4 }));

            //Console.WriteLine(gameOfThrones("aaabbbb"));
            //Console.WriteLine(gameOfThrones("cdefghmnopqrstuvw"));
            //Console.WriteLine(gameOfThrones("cdcdcdcdeeeef"));

            //Console.WriteLine(queensAttack2(7, 3, 4, 3, new int[][] {
            //    //new int[] { 5, 5 },
            //    //new int[] { 4, 2 },
            //    //new int[] { 2, 3 },
            //    new int[] { 2, 5 },
            //    new int[] { 5, 2 }
            //}));

            //Console.WriteLine(equalizeArray(new int[] { 3, 3, 2, 1, 3 }));

            //Console.WriteLine(repeatedString("aba", 10));

            //Console.WriteLine(beautifulDays(20, 23, 6));

            //Console.WriteLine(string.Join("\n\r", cutTheSticks(new int[] { 23, 74, 26, 23, 92, 92, 44, 13, 34, 23, 69, 4, 19, 94, 94, 38, 14, 9, 51, 98, 72, 46, 17, 25, 21, 87, 99, 50, 59, 53, 82, 24, 93, 16, 88, 52, 14, 38, 27, 7, 18, 81, 13, 75, 80, 11, 29, 39, 37, 78, 55, 17, 78, 12, 77, 84, 63, 29, 68, 32, 17, 55, 31, 30, 3, 17, 99, 6, 45, 81, 75, 31, 50, 93, 66, 98, 94, 59, 68, 30, 98, 57, 83, 75, 68, 85, 98, 76, 91, 23, 53, 42, 72, 77 })));
            //Console.WriteLine(string.Join(", ", cutTheSticks(new int[] { 5, 4, 4, 2, 2, 8 })));

            //Console.WriteLine(appendAndDelete("aaa", "a", 5));  // Yes
            //Console.WriteLine(appendAndDelete("abcd", "abcdert", 10));  // No
            //Console.WriteLine(appendAndDelete("hackerhappy", "hackerrank", 9));

            //Console.WriteLine(findDigits(12));

            //Console.WriteLine(angryProfessor(3, new int[] { -1, -3, 4, 2 }));
            //Console.WriteLine(angryProfessor(2, new int[] { 0, -1, 2, 1 }));

            //Console.WriteLine(biggerIsGreater.run("ab"));
            //Console.WriteLine(biggerIsGreater.run("bb"));
            //Console.WriteLine(biggerIsGreater.run("hefg"));
            //Console.WriteLine(biggerIsGreater.run("dhck"));
            //Console.WriteLine(biggerIsGreater.run("dkhc"));

            //Console.WriteLine(utopianTree(4));
            //Console.WriteLine(utopianTree(3));
            //Console.WriteLine(utopianTree(0));
            //Console.WriteLine(utopianTree(1));

            //Console.WriteLine(designerPdfViewer(new int[] { 1, 3, 1, 3, 1, 4, 1, 3, 2, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, }, "abc"));

            //Console.WriteLine(pickingNumbers(new int[] { 4, 97, 5, 97, 97, 4, 97, 4, 97, 97, 97, 97, 4, 4, 5, 5, 97, 5, 97, 99, 4, 97, 5, 97, 97, 97, 5, 5, 97, 4, 5, 97, 97, 5, 97, 4, 97, 5, 4, 4, 97, 5, 5, 5, 4, 97, 97, 4, 97, 5, 4, 4, 97, 97, 97, 5, 5, 97, 4, 97, 97, 5, 4, 97, 97, 4, 97, 97, 97, 5, 4, 4, 97, 4, 4, 97, 5, 97, 97, 97, 97, 4, 97, 5, 97, 5, 4, 97, 4, 5, 97, 97, 5, 97, 5, 97, 5, 97, 97, 97 }));
            //Console.WriteLine(pickingNumbers(new int[] { 4, 6, 5, 3, 3, 1 }));
            //Console.WriteLine(pickingNumbers(new int[] { 1, 2, 2, 3, 1, 2 }));

            //Console.WriteLine(catAndMouse(1, 2, 3));
            //Console.WriteLine(catAndMouse(1, 3, 2));

            //Console.WriteLine(solve(73201, 57075));
            //Console.WriteLine(solve(5, 4));

            //Console.WriteLine(sockMerchant(9, new int[] { 10, 20, 20, 10, 10, 30, 50, 10, 20 }));

            //Console.WriteLine(getMoneySpent(new int[] { 3, 1 }, new int[] { 5, 2, 8 }, 10));
            //Console.WriteLine(getMoneySpent(new int[] { 4 }, new int[] { 5 }, 5));

            //Console.WriteLine(countingValleys(8, "UDDDUDUU"));

            //bonAppetit(1, new int[] { 3, 10, 2, 9 }, 7);
            //bonAppetit(1, new int[] { 3, 10, 2, 9 }, 12);

            //Console.WriteLine(dayOfProgrammer(1800));
            //Console.WriteLine(dayOfProgrammer(2017));
            //Console.WriteLine(dayOfProgrammer(2016));
            //Console.WriteLine(dayOfProgrammer(1918));

            //var mb0 = migratoryBirds(6, new int[] { 1, 4, 4, 4, 5, 3 });

            //var dsp0 = divisibleSumPairs(6, 3, new int[] { 1, 3, 2, 6, 1, 2 });
            //var dsp1 = divisibleSumPairs(100, 22, new int[] { 71, 44, 2, 93, 66, 27, 41, 99, 49, 68, 60, 16, 45, 21, 71, 96, 89, 91, 60, 21, 43, 9, 56, 48, 25, 96, 91, 99, 73, 22, 48, 32, 27, 71, 72, 90, 9, 62, 68, 70, 77, 98, 2, 32, 69, 51, 99, 35, 47, 83, 82, 43, 87, 47, 40, 54, 53, 85, 78, 31, 98, 26, 56, 100, 88, 43, 77, 81, 58, 31, 46, 70, 57, 8, 16, 53, 8, 61, 22, 62, 75, 94, 91, 29, 95, 69, 22, 12, 88, 5, 87, 90, 10, 86, 86, 85, 73, 95, 87, 53 });

            //var solve0 = solve(5, new int[] { 1, 2, 1, 3, 2 }, 3, 2);
            //var solve1 = solve(6, new int[] { 1, 1, 1, 1, 1, 1 }, 3, 2);
            //var solve2 = solve(1, new int[] { 4 }, 4, 1);

            //var br1 = breakingRecords(new int[] { 10, 5, 20, 20, 4, 5, 2, 25, 1 });
            //var br2 = breakingRecords(new int[] { 3, 4, 21, 36, 10, 28, 35, 5, 24, 42 });

            //Console.WriteLine(MagicSquare.formingMagicSquare(new int[][] {
            //    new int[] { 4, 8, 2 },
            //    new int[] { 4, 5, 7 },
            //    new int[] { 6, 1, 6 }
            //}));

            //Console.WriteLine(getTotalX(new int[] { 2, 4 }, new int[] { 16, 32, 96 }));

            //Console.WriteLine(kangaroo(0, 3, 4, 2));
            //Console.WriteLine(kangaroo(0, 2, 5, 3));

            //countApplesAndOranges(7, 11, 5, 15, new int[] { -2, 2, 1, 6 }, new int[] { 5, -6, -15 });
        }

        static string fairRations(int[] B)
        {
            // Complete this function

            var loafs = B
                .Select(b => b % 2)
                .ToList();

            var pos = 0;
            var breads = 0;

            while (pos < loafs.Count)
            {
                var first = loafs.IndexOf(1);

                if (first > -1)
                {
                    pos = first + 1;

                    loafs[first] ^= 1;
                    breads++;

                    if (first + 1 < loafs.Count)
                    {
                        loafs[first + 1] ^= 1;
                        breads++;
                    }
                    else
                    {
                        loafs[first - 1] ^= 1;
                        breads++;
                    }
                }
                else
                {
                    break;
                }
            }

            if (loafs.Any(l => l == 1))
                return "NO";

            return breads.ToString();
        }

        static int workbook(int n, int k, int[] arr)
        {
            // Complete this function

            var specials = 0;
            var page = 1;

            for (var i = 0; i < n; i++)
            {
                var problems = arr[i];
                var totInPage = 0;
                for (var p = 1; p < problems + 1; p++)
                {
                    if (totInPage == k)
                    {
                        page++;
                        totInPage = 0;
                    }

                    totInPage++;

                    if (p == page)
                        specials++;
                }

                page++;
            }

            return specials;
        }

        static int chocolateFeast(int n, int c, int m)
        {
            // Complete this function

            if (n == 0)
                return 0;

            var tot = n / c;
            var wrappers = tot;

            while (wrappers >= m)
            {
                var buy = wrappers / m;
                var left = wrappers % m;

                tot += buy;
                wrappers = left + buy;
            }

            return tot;
        }

        static string[] cavityMap(string[] grid)
        {
            // Complete this function

            Func<int, int, int> getValue = (int x, int y) =>
            {
                return int.Parse(grid[x][y].ToString());
            };

            var n = grid.Length;
            var tr = (string[])grid.Clone();

            for (var x = 1; x < n - 1; x++)
            {
                for (var y = 1; y < n - 1; y++)
                {
                    var t = getValue(x, y);

                    //if ((t >= getValue(x - 1, y - 1)) && (t >= getValue(x - 1, y)) && (t >= getValue(x - 1, y + 1))
                    //    && (t >= getValue(x, y - 1)) && (t >= getValue(x, y + 1))
                    //    && (t >= getValue(x + 1, y - 1)) && (t >= getValue(x + 1, y)) && (t >= getValue(x + 1, y + 1)))
                    //    tr[x] = tr[x].Substring(0, y) + "X" + tr[x].Substring(y + 1);

                    if ((t > getValue(x - 1, y))
                        && (t > getValue(x, y - 1)) && (t > getValue(x, y + 1))
                        && (t > getValue(x + 1, y)))
                        tr[x] = tr[x].Substring(0, y) + "X" + tr[x].Substring(y + 1);
                }
            }

            return tr;
        }

        static int[] kaprekarNumbers(int p, int q)
        {
            // Complete this function

            var min = Math.Min(p, q);
            var max = Math.Max(p, q);

            Func<int, bool> isKaprekar = (int num) =>
            {
                var d = num.ToString().Length;
                var square = ((long)num * num).ToString();

                var ls = square.Substring(0, square.Length - d);
                var rs = square.Substring(ls.Length);

                var l = 0;
                int.TryParse(ls, out l);
                var r = 0;
                int.TryParse(rs, out r);

                return l + r == num;
            };

            return Enumerable.Range(min, max - min + 1)
                .Where(num => isKaprekar(num))
                .ToArray();
        }

        static int saveThePrisoner(int n, int m, int s)
        {
            // Complete this function
            // 0 1

            int a = s + m - 1;
            if (a > n)
            {
                if (a % n == 0)
                {
                    return n;
                }
                return a % n;
            }

            return a;
        }

        static int viralAdvertising(int n)
        {
            // Complete this function

            var likes = 0;
            var recipients = 5;

            for (var i = 0; i < n; i++)
            {
                var thisLikes = recipients / 2;

                likes += thisLikes;

                recipients = thisLikes * 3;
            }

            return likes;
        }

        static int flatlandSpaceStations(int n, int[] c)
        {
            // Complete this function

            int mostLarge = -1;
            var cadj = c
                .OrderBy(a => a)
                .ToArray();

            for (var i = 0; i < cadj.Length - 1; i++)
            {
                var thisDistance = cadj[i + 1] - cadj[i];
                if (thisDistance > mostLarge)
                    mostLarge = thisDistance;
            }

            mostLarge /= 2;

            var firstDistance = cadj.First();
            if (firstDistance > mostLarge)
                mostLarge = firstDistance;

            var lastDistance = n - cadj.Last() - 1;
            if (lastDistance > mostLarge)
                mostLarge = lastDistance;

            return mostLarge;
        }

        static int beautifulTriplets(int d, int[] arr)
        {
            // Complete this function

            var beats = 0;

            for (var i = 0; i < arr.Length; i++)
            {
                for (var j = i + 1; j < arr.Length; j++)
                {
                    if (arr[j] - arr[i] == d)
                    {
                        for (var k = j + 1; k < arr.Length; k++)
                        {
                            if (arr[k] - arr[j] == d)
                                beats++;
                        }
                    }
                }
            }

            return beats;
        }

        static int[] acmTeam(string[] topic)
        {
            // Complete this function

            int known, max_known = 0, know_all = 0;

            for (int i = 0; i < topic.Length - 1; i++)
            {
                for (int j = i + 1; j < topic.Length; j++)
                {
                    known = 0;
                    for (int k = 0; k < topic[i].Length; k++)
                    {
                        if (topic[i][k] == '1' || topic[j][k] == '1')
                            known++;
                        if (max_known < known)
                        {
                            max_known = known;
                            know_all = 0;
                        }
                        if (known == max_known)
                            know_all++;
                    }
                }
            }

            return new int[] { max_known, know_all };
        }

        static ulong taumBday(int b, int w, int bc, int wc, int z)
        {
            /*
             * Write your code here.
             */

            if (z >= Math.Abs(bc - wc))
            {
                return (ulong)(b * bc) + (ulong)(w * wc);
            }
            else if (bc <= wc)
            {
                return (ulong)((b + w) * bc) + (ulong)(w * z);
            }
            else
            {
                return (ulong)((b + w) * wc) + (ulong)(b * z);
            }

            //bc = bc > wc ? ((bc - wc > z) ? wc + z : bc) : bc;
            //wc = wc > bc ? ((wc - bc > z) ? bc + z : wc) : wc;
            //return (ulong)(b * bc) + (ulong)(w * wc);

            //if (bc == wc)
            //{
            //    return (b * bc) + (w * wc);
            //}

            //var gifs = new int[][]
            //{
            //    new int[] { b, bc },
            //    new int[] { w, wc }
            //}
            //    .OrderBy(n => n[1])
            //    .ToArray();

            //if (gifs[0][1] + z < gifs[1][1])
            //{
            //    var chepest = (long)gifs[0][1] * gifs[0][0];
            //    var highest = (long)(gifs[0][1] + z) * gifs[1][0];
            //    var total = chepest + highest;
            //    return total;
            //}
            //else
            //{
            //    return (b * bc) + (w * wc);
            //}
        }

        static string twoArrays(int k, int[] A, int[] B)
        {
            // Complete this function

            var acres = A
                .OrderBy(a => a)
                .ToArray();

            var bdescre = B
                .OrderBy(b => b)
                .ToList();

            for (var i = 0; i < A.Length; i++)
            {
                var n = acres[i];

                var diff = k - n;

                var has = bdescre
                    .Where(b => b >= diff)
                    .ToArray();

                if (!has.Any())
                    return "NO";
                else
                {
                    bdescre.Remove(has[0]);
                }
            }

            return "YES";
        }

        static string gameOfThrones(string s)
        {
            // Complete this function

            var gs = s
                .GroupBy(c => c)
                .Select(c => new { c = c.Key, len = c.Count() })
                .ToList();

            if (s.Length % 2 == 1)
            {
                var hasJustOne = gs
                    .FirstOrDefault(g => g.len % 2 == 1);

                if (hasJustOne == null)
                    return "NO";

                gs.Remove(hasJustOne);
            }

            var hasOneNotImpar = gs
                .Where(g => g.len % 2 == 1)
                .Any();

            if (hasOneNotImpar)
                return "NO";
            else
                return "YES";
        }

        static int queensAttack2(int n, int k, int r_q, int c_q, int[][] obstacles)
        {
            // Complete this function
            // n = length of board
            // k = count of obstacles

            var posAttack = 0;

            var obts = obstacles
                .Select(o => new { r = o[0] - r_q, c = o[1] - c_q })
                .ToArray();

            Action<int, bool, int> checkAndAddRowCol = delegate (int refer, bool isrow, int dir)
            {
                var thisObts = obts
                    .Where(o => isrow ? o.c == 0 && (o.r * dir) > 0 : o.r == 0 && (o.c * dir) > 0)
                    .Select(o => Math.Abs(isrow ? o.r : o.c))
                    .OrderBy(o => o)
                    .ToArray();

                if (thisObts.Any())
                {
                    posAttack += thisObts.First() - 1;
                }
                else
                {
                    posAttack += refer;
                }
            };

            checkAndAddRowCol(n - r_q, true, 1);
            checkAndAddRowCol(r_q - 1, true, -1);
            checkAndAddRowCol(n - c_q, false, 1);
            checkAndAddRowCol(c_q - 1, false, -1);

            var mtr = Math.Min(n - r_q, n - c_q);
            var mtl = Math.Min(n - r_q, c_q - 1);
            var mdr = Math.Min(r_q - 1, n - c_q);
            var mdl = Math.Min(r_q - 1, c_q - 1);

            Action<int, int, int> checkAndAdd = delegate (int refer, int rdir, int cdir)
            {
                var thisObts = obts
                    .Where(o => o.r * rdir == o.c * cdir && o.c * cdir > 0)
                    .Select(o => Math.Abs(o.c))
                    .OrderBy(o => o)
                    .ToArray();

                if (thisObts.Any())
                {
                    posAttack += thisObts.First() - 1;
                }
                else
                {
                    posAttack += refer;
                }
            };

            checkAndAdd(mtr, 1, 1);
            checkAndAdd(mtl, 1, -1);
            checkAndAdd(mdr, -1, 1);
            checkAndAdd(mdl, -1, -1);

            return posAttack;
        }

        static int equalizeArray(int[] arr)
        {
            // Complete this function

            var groups = arr
                .GroupBy(a => a)
                .Select(a => a.Count())
                .ToArray();

            var largGroup = groups
                .Max();

            return arr.Length - largGroup;
        }

        static long repeatedString(string s, long n)
        {
            // Complete this function n >= 1

            if (n <= s.Length)
            {
                return s.Substring(0, (int)n)
                    .Where(sc => sc == 'a')
                    .Count();
            }

            var ains = s
                    .Where(sc => sc == 'a')
                    .Count();

            var rep = n / s.Length;
            var falt = n % s.Length;

            var tr = rep * ains;
            tr += s.Substring(0, (int)falt)
                    .Where(sc => sc == 'a')
                    .Count();

            return tr;
        }

        static int beautifulDays(int i, int j, int k)
        {
            // Complete this function

            var nums = Enumerable.Range(i, j - i + 1)
                .Select(n => new { num = n, inv = int.Parse(new string(n.ToString().Reverse().ToArray())) })
                .ToArray();

            var beautDays = nums
                .Where(n => Math.Abs(n.num - n.inv) % k == 0)
                .ToArray();

            return beautDays
                .Length;
        }

        static int[] cutTheSticks(int[] arr)
        {
            // Complete this function

            var ret = new List<int>();
            var tocut = (int[])arr.Clone();

            while (tocut.Any())
            {
                ret.Add(tocut.Length);

                var small = tocut.Min();

                var cuted = tocut
                    .Select(t => {
                        var p0 = t - small;
                        if (p0 == 0)
                            return -1;
                        else
                            return p0;
                    })
                    .Where(n => n > 0)
                    .ToArray();

                tocut = (int[])cuted.Clone();
            }

            return ret.ToArray();
        }

        static string appendAndDelete(string s, string t, int k)
        {
            // Complete this function s to t

            var commum = "";
            var minor = Math.Min(s.Length, t.Length);

            for (var i = 0; i < minor; i++)
            {
                if (s[i] == t[i])
                {
                    commum += s[i];
                }
                else
                {
                    break;
                }
            }

            if ((s.Length + t.Length - 2 * commum.Length) > k)
            {
                return "No";
            }
            else if ((s.Length + t.Length - 2 * commum.Length) % 2 == k % 2)
            {
                return  "Yes";
            }
            else if ((s.Length + t.Length - k) < 0)
            {
                return "Yes";
            }
            else
            {
                return "No";
            }

            //steps += s.Length - commum.Length;
            //steps += t.Length - commum.Length;

            //if (steps < k)
            //{
            //    if ((k - steps) % 2 == 0)
            //        steps = k;
            //}

            //return steps == k ?
            //    "Yes" :
            //    "No";
        }

        static int findDigits(int n)
        {
            // Complete this function

            var str = n.ToString();
            var count = 0;

            foreach (var c in str)
            {
                var num = int.Parse(c.ToString());
                if (num > 0)
                    if (n % num == 0)
                        count++;
            }

            return count;
        }

        static int[] circularArrayRotation(int[] a, int[] m)
        {
            // Complete this function

            var list = a.ToList();
            var lastV = a.Length - 1;

            for (var rot = 0; rot < a.Length - 1; rot++)
            {
                var atr = list[lastV];
                list.RemoveAt(lastV);
                list.Insert(0, atr);
            }

            return m
                .Select(mm => list[mm])
                .ToArray();
        }

        static string angryProfessor(int k, int[] a)
        {
            // Complete this function

            var arrived = a
                .Where(aa => aa <= 0)
                .Count();

            return arrived >= k ?
                "NO" :
                "YES";
        }

        static int utopianTree(int n)
        {
            // Complete this function

            var tall = 1;

            var seasons = Enumerable.Range(1, n)
                .Select(num => num % 2 == 0 ? "sum" : "spring")
                .ToArray();

            foreach (var season in seasons)
            {
                switch(season)
                {
                    case "sum": tall += 1; break;
                    case "spring": tall *= 2; break;
                }
            }

            return tall;
        }

        static int designerPdfViewer(int[] h, string word)
        {
            // Complete this function

            var length = word.Length;

            var height = System.Text.Encoding.ASCII.GetBytes(word.ToUpper())
                .Select(w => h[w - 65]);

            return length * height.Max();
        }

        static int hurdleRace(int k, int[] height)
        {
            // Complete this function

            var max = height
                .Max();

            var refris = max - k;

            return refris < 0 ?
                0 :
                refris;
        }

        static int pickingNumbers(int[] a)
        {
            // Complete this function

            var dist = a
                .GroupBy(aa => aa)
                .Select(aa => new { key = aa.Key, count = aa.Count() })
                .OrderBy(aa => aa.key)
                .ToArray();

            var tr = dist[0].count;

            for (var rf = 0; rf < dist.Length - 1; rf++)
            {
                var tot = dist[rf].count;

                if (Math.Abs(dist[rf].key - dist[rf + 1].key) <= 1)
                    tot += dist[rf + 1].count;

                if (tot > tr)
                    tr = tot;
            }

            return tr;
        }

        static string catAndMouse(int x, int y, int z)
        {
            /*
             * Write your code here.
             */

            var catAtoMouse = Math.Abs(z - x);

            var catBtoMouse = Math.Abs(z - y);

            if (catAtoMouse == catBtoMouse)
                return "Mouse C";
            else if (catAtoMouse < catBtoMouse)
                return "Cat A";
            else
                return "Cat B";
        }

        static int solve(int n, int p)
        {
            // Complete this function

            var fromStart = Math.Ceiling((decimal)(p - 1) / 2);

            var lastPageInFrontSignal = n % 2;

            var fromEnd = Math.Abs(Math.Ceiling((decimal)(n - p - lastPageInFrontSignal) / 2));

            return (int)Math.Min(fromStart, fromEnd);
        }

        static int sockMerchant(int n, int[] ar)
        {
            // Complete this function
            var groups = ar
                .GroupBy(a => a)
                .ToArray();

            var count = groups
                .Sum(a => (int)a.Count() / 2);

            return count;
        }

        static int getMoneySpent(int[] keyboards, int[] drives, int s)
        {
            // Complete this function
            var allPossible = keyboards
                .SelectMany(k => drives.Select(d => new { key = k, drive = d, total = k + d }))
                .ToArray();

            var sheCanBuy = allPossible
                .Where(a => a.total <= s)
                .OrderByDescending(a => a.total)
                .ToArray();

            if (sheCanBuy.Any())
            {
                return sheCanBuy.First().total;
            }
            else
            {
                return -1;
            }
        }

        static int countingValleys(int n, string s)
        {
            // Complete this function
            var level = 0;
            var valleys = 0;
            foreach (var c in s)
            {
                if (c == 'U')
                {
                    if (level == -1)
                        valleys++;

                    level++;
                }
                else
                {
                    level--;
                }
            }
            return valleys;
        }

        static void bonAppetit(int k, int[] ar, int charge)
        {
            var fullBill = ar.Sum();

            var billWithoutK = fullBill - ar[k];

            var halfBill = billWithoutK / 2;

            if (halfBill == charge)
            {
                Console.WriteLine("Bon Appetit");
            }
            else
            {
                var diff = charge - halfBill;
                Console.WriteLine(diff.ToString());
            }
        }

        static void bonAppetitProgram()
        {
            /* Enter your code here. Read input from STDIN. Print output to STDOUT */
            var nk = ReadInts();
            var k = nk[1];

            var ar = ReadInts();

            var charge = ReadInt();

            bonAppetit(k, ar, charge);
        }

        static string dayOfProgrammer(int year)
        {
            // Complete this function
            var daysUntilAgo = 215;

            var isLeap = ((year % 4 == 0 && year % 100 != 0) || (year % 400 == 0));

            var totalDays = daysUntilAgo + (isLeap ? 29 : 28);

            if (year == 1918)
            {
                totalDays -= 13;

                var day = 256 - totalDays;

                return $"{day.ToString("00")}.09.1918";
            }
            else
            {
                var day = 256 - totalDays;

                return $"{day.ToString("00")}.09.{year.ToString()}";
            }
        }

        static int migratoryBirds(int n, int[] ar)
        {
            // Complete this function
            var at = ar
                .GroupBy(a => a)
                .Select(g => new { id = g.Key, count = g.Count() })
                .ToArray();

            var topCount = at
                .OrderByDescending(a => a.count)
                .First();

            var allWithThisCount = at
                .Where(a => a.count == topCount.count)
                .OrderBy(a => a.id)
                .ToArray();

            return allWithThisCount
                .First()
                .id;
        }

        static int divisibleSumPairs(int n, int k, int[] ar)
        {
            // Complete this function
            var tr = 0;
            for (var i = 0; i < n - 1; i++)
            {
                for (var t = i + 1; t < n; t++)
                {
                    if ((ar[i] + ar[t]) % k == 0)
                        tr++;
                }
            }
            return tr;
        }

        static int solve(int n, int[] s, int d, int m)
        {
            // Complete this function
            var count = 0;
            for (var c = 0; c < n; c++)
            {
                if (c + m > n)
                    continue;

                var tcount = 0;
                for (var tc = 0; tc < m; tc++)
                {
                    tcount += s[c + tc];
                }

                if (tcount == d)
                    count++;
            }
            return count;
        }


        static int[] breakingRecords(int[] score)
        {
            /*
             * Write your code here.
             */
            var high = score[0];
            var low = score[0];
            var breakHigh = 0;
            var breakLow = 0;

            for (var i = 1; i < score.Length; i++)
            {
                if (score[i] > high)
                {
                    breakHigh++;
                    high = score[i];
                }
                if (score[i] < low)
                {
                    breakLow++;
                    low = score[i];
                }
            }

            return new int[] { breakHigh, breakLow };
        }

        static int getTotalX(int[] a, int[] b)
        {
            /*
             * Write your code here.
             */
            //var factors = new List<int>();
            var factors = a.SelectMany(aa => a.Select(aaa => aaa * aa))
                .Distinct()
                .ToArray();

            var rt = 0;



            return rt;
        }

        static string kangaroo(int x1, int v1, int x2, int v2)
        {
            // x1 = kangarro 1 position
            // v1 = kangarro 1 jumper length
            // Complete this function

            var isX1first = x1 > x2;
            var isX1faster = v1 > v2;

            if (!(isX1first ^ isX1faster))
                return "NO";

            int atX1 = x1, atX2 = x2;
            var awser = "";

            do
            {
                atX1 += v1;
                atX2 += v2;

                if (atX1 == atX2)
                    awser = "YES";
                else
                {
                    if (isX1first)
                    {
                        if (atX2 > atX1)
                            awser = "NO";
                    }
                    else
                    {
                        if (atX1 > atX2)
                            awser = "NO";
                    }
                }

            } while (awser == "");

            return awser;
        }


        static void countApplesAndOranges(int s, int t, int a, int b, int[] apples, int[] oranges)
        {
            /*   5      7    11         15
             *   a      s    t          b
             * Write your code here.
             */

            var applesIn = apples
                .Where(ap => (a + ap) >= s && (a + ap) <= t)
                .ToArray();

            var orangesIn = oranges
                .Where(or => (b + or) <= t && (b + or) >= s)
                .ToArray();

            Console.WriteLine(applesIn.Length.ToString());
            Console.WriteLine(orangesIn.Length.ToString());
        }
    }
}
