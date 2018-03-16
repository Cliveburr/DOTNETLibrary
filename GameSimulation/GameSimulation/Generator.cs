using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameSimulation
{
    public abstract class Generator
    {
        protected Random _rnd;
        protected Game _game;

        public Generator(Game game)
        {
            _rnd = new Random(DateTime.Now.Millisecond);
            _game = game;
        }

        public abstract Ticket GenerateSingle(int count);

        public abstract Ticket[] GenerateDistinct(int ticketCount, int differ, int count);

        protected int GenDistinctNumber(Func<int, bool> condition)
        {
            int n = 0;
            do
            {
                n = _rnd.Next(_game.MinimumNumber, _game.MaximumNumber + 1);
            } while (condition(n));
            return n;
        }

        protected int[] GenDistinctNumbers(int count, Func<List<int>, bool> condition)
        {
            var list = new List<int>();
            do
            {
                list.Clear();
                for (var i = 0; i < count; i++)
                {
                    var n = GenDistinctNumber(na => list.Contains(na));
                    list.Add(n);
                }
            } while (condition(list));
            return list.ToArray();
        }
    }
}