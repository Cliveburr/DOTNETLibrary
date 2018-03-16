using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameSimulation
{
    public class Bet
    {
        public List<BetItem> Bets { get; set; }

        public double Cost
        {
            get
            {
                var total = 0.0;
                foreach (var bet in Bets)
                {
                    total += bet.Type.Cost;
                }
                return total;
            }
        }
    }

    public class BetItem
    {
        public BetType Type { get; set; }
        public Ticket Ticket { get; set; }
    }

    public class BetType
    {
        public int Count { get; set; }
        public double Cost { get; set; }
    }

    public class BetResult
    {
        public Ticket Lottery { get; set; }
        public List<BetItemResult> Bets { get; set; }

        public double TotalReward
        {
            get
            {
                var total = 0.0;
                foreach (var bet in Bets)
                {
                    total += bet.Reward?.Value ?? 0.0;
                }
                return total;
            }
        }
    }

    public class BetItemResult
    {
        public BetItem Item { get; set; }
        public int[] Result { get; set; }
        public GameReward Reward { get; set; }

        public bool IsWinner
        {
            get
            {
                return Reward != null;
            }
        }
    }
}