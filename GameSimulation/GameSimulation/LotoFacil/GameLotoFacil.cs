using GameSimulation.Generators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameSimulation.LotoFacil
{
    public class GameLotoFacil : Game
    {
        public BetType Bet15 { get; set; }
        public BetType Bet16 { get; set; }
        public BetType Bet17 { get; set; }
        public BetType Bet18 { get; set; }


        public GameLotoFacil()
        {
            MinimumNumber = 1;
            MaximumNumber = 25;
            LotteryGenerator = new RandomGenerator(this);
            LotteryCount = 15;
            Bet15 = new BetType { Count = 15, Cost = 2.00 };
            Bet16 = new BetType { Count = 16, Cost = 32.00 };
            Bet17 = new BetType { Count = 15, Cost = 272.00 };
            Bet18 = new BetType { Count = 15, Cost = 1632.00 };
            Rewards = new List<GameReward>
            {
                new GameReward { ForHits = 11, Value = 4.00 },
                new GameReward { ForHits = 12, Value = 8.00 },
                new GameReward { ForHits = 13, Value = 20.00 },
                new GameReward { ForHits = 14, Value = 1800.00 },
                new GameReward { ForHits = 15, Value = 450000.00 }
            };
        }
    }
}