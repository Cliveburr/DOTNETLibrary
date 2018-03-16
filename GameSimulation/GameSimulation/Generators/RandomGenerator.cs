using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameSimulation.Generators
{
    public class RandomGenerator : Generator
    {
        public RandomGenerator(Game game)
            : base(game)
        {
        }

        public override Ticket GenerateSingle(int count)
        {
            var numbers = new List<int>();
            for (var i = 0; i < count; i++)
            {
                var newNumber = 0;
                do
                {
                    newNumber = _rnd.Next(_game.MinimumNumber, _game.MaximumNumber + 1);
                }
                while (numbers.Contains(newNumber));
                numbers.Add(newNumber);
            }
            return new Ticket
            {
                Numbers = numbers.OrderBy(n => n).ToArray()
            };
        }

        public override Ticket[] GenerateDistinct(int ticketCount, int differ, int count)
        {
            var tickets = new List<Ticket>();
            for (var i = 0; i < count; i++)
            {
                Ticket newTicket = null;
                do
                {
                    newTicket = GenerateSingle(count);
                } while (tickets.Where(t => t.Numbers.Intersect(newTicket.Numbers).Count() > differ).Any());
                tickets.Add(newTicket);
            }
            return tickets.ToArray();
        }
    }
}