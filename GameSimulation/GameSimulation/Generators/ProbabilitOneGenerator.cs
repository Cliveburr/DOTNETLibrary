using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameSimulation.Generators
{
    public class ProbabilitOneGenerator : RandomGenerator
    {
        public ProbabilitOneGenerator(Game game)
            : base(game)
        {
        }

        public override Ticket[] GenerateDistinct(int ticketCount, int differ, int count)
        {
            throw new Exception();
        }

        public Ticket[] GenerateSomes(int topCount, int lowCount, int ticketCount, int count)
        {
            var c = 0;
            var lastTen = _game.History
                .Select(g => new { id = c++, game = g })
                .OrderByDescending(g => g.id)
                .Take(10)
                .Select(g => g.game)
                .ToList();

            var decs = Enumerable.Range(_game.MinimumNumber, _game.MaximumNumber)
                .Select(na =>
                    new
                    {
                        number = na,
                        count = lastTen
                            .Where(lt => lt.Numbers.Contains(na))
                            .Count()
                    })
                 .ToList();

            var topNine = decs.ToList()
                .OrderByDescending(d => d.count)
                .Take(topCount);

            var lowerFive = decs.ToList()
                .OrderBy(d => d.count)
                .Take(lowCount);

            var numbers = topNine
                .Concat(lowerFive)
                .Select(na => na.number)
                .ToList();

            var usedCombinations = new List<int[]>();
            var tickets = new List<Ticket>();
            var left = count - topCount - lowCount;
            for (var i = 0; i < ticketCount; i++)
            {
                //int n = 0;
                //do
                //{
                //    n = _rnd.Next(_game.MinimumNumber, _game.MaximumNumber + 1);
                //} while (numbers.Contains(n) || usedNums.Contains(n));
                int[] newn = GenDistinctNumbers(left, (ns) =>
                    numbers.Intersect(ns).Count() != 0 ||
                    usedCombinations
                    .Where(uc => uc.Intersect(ns).Count() == left)
                    .Any());

                usedCombinations.Add(newn);

                var finalNumbers = numbers
                        .Concat(newn);

                if (finalNumbers.GroupBy(x => x)
                        .Where(g => g.Count() > 1)
                        .Any())
                    throw new Exception();

                tickets.Add(new Ticket
                {
                    Numbers = finalNumbers
                        .OrderBy(na => na)
                        .ToArray()
                });
            }
            return tickets.ToArray();
        }
    }


}