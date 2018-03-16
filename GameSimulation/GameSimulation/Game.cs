using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameSimulation
{
    public abstract class Game
    {
        public int MinimumNumber { get; set; }
        public int MaximumNumber { get; set; }
        public List<Ticket> History { get; set; }
        public Generator LotteryGenerator { get; set; }
        public int LotteryCount { get; set; }
        public List<GameReward> Rewards { get; set; }

        public Game()
        {
            History = new List<Ticket>();
        }

        public Ticket MakeNewLottery()
        {
            var ticket = LotteryGenerator.GenerateSingle(LotteryCount);
            History.Add(ticket);
            return ticket;
        }

        public BetResult CheckWinner(Bet bet, Ticket lottery)
        {
            var result = new BetResult
            {
                Lottery = lottery,
                Bets = new List<BetItemResult>()
            };
            foreach (var item in bet.Bets)
            {
                var itemResult = new BetItemResult
                {
                    Item = item,
                    Result = item.Ticket.Numbers.Intersect(lottery.Numbers).ToArray()
                };

                itemResult.Reward = Rewards.Where(r => r.ForHits == itemResult.Result.Length).FirstOrDefault();

                result.Bets.Add(itemResult);
            }
            return result;
        }
    }

    public class GameReward
    {
        public int ForHits { get; set; }
        public double Value { get; set; }
    }
}