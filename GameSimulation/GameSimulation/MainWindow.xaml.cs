using GameSimulation.Generators;
using GameSimulation.LotoFacil;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace GameSimulation
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            TestOldResults();
            return;

            var lotoFacil = new GameLotoFacil();
            var gen = new RandomGenerator(lotoFacil);
            var prob = new ProbabilitOneGenerator(lotoFacil);
            var sb = new StringBuilder();

            for (var i = 0; i < 100; i++)
            {
                lotoFacil.MakeNewLottery();
            }

            var mimBet = 70.0;
            var walet = 0.0;
            var totalCost = 0.0;
            var totalReward = 0.0;
            for (var i = 0; i < 10000; i++)
            {
                walet += mimBet;
                var totalBet = (int)Math.Floor(walet / lotoFacil.Bet15.Cost);

                var topCount = 0;
                var lowCount = 0;
                if (totalBet < 12)  // 15 - 9 - 5 = 1 ## 25 - 14 = 11
                {
                    topCount = 9; lowCount = 5;
                }
                else if (totalBet > 12 && totalBet < 144) // 8 + 5 = 13 - 15 = 2  ##  25 - 13 = 12 ^ 2 = 144
                {
                    topCount = 8; lowCount = 5;
                }
                else if (totalBet > 144 && totalBet < 2197) // 8 + 4 = 12 - 15 = 3  ## 25 - 12 = 13 ^ 3 = 2197
                {
                    topCount = 8; lowCount = 4;
                }
                else if (totalBet > 2197 && totalBet < 38416) // 7 + 4 = 11 - 15 = 4  ## 25 - 11 = 14 ^ 4 = 38416
                {
                    topCount = 7; lowCount = 4;
                }
                else
                {
                    throw new Exception();
                }
                //else if (totalBet > 57 && totalBet < 76) // 7 + 3 = 10 - 15 = 5  ## 25 - 10 = 15 * 5 = 75
                //{
                //    topCount = 7; lowCount = 3;
                //}
                //else if (totalBet > 76 && totalBet < 97) // 6 + 3 = 9 - 15 = 6  ## 25 - 9 = 16 * 6 = 96
                //{
                //    topCount = 6; lowCount = 3;
                //}
                //else
                //{
                //    topCount = 6; lowCount = 2;
                //}

                var bet = new Bet
                {
                    Bets = prob.GenerateSomes(topCount, lowCount, totalBet, lotoFacil.Bet15.Count).Select(t => new BetItem
                    {
                        Type = lotoFacil.Bet15,
                        Ticket = t
                    }).ToList()
                };
                totalCost += bet.Cost;

                var lottery = lotoFacil.MakeNewLottery();

                var result = lotoFacil.CheckWinner(bet, lottery);

                totalReward += result.TotalReward;

                //var text = $"Try {(i + 1).ToString("N1").PadLeft(5)} - Cost = {bet.Cost} - Reward = {result.TotalReward} - TotalCost = {totalCost} - TotalReward = {totalReward}";
                var text = string.Format("Try {0,5:#} - Walet = {1,8:###,0} Bets = {2,5:#} - Cost = {3,8:###,0} - Reward = {4,8:###,0} - TotalCost = {5,8:###,0} - TotalReward = {6,8:###,0}", i + 1, walet, bet.Bets.Count(), bet.Cost, result.TotalReward, totalCost, totalReward);
                Console.WriteLine(text);
                sb.AppendLine(text);

                walet -= bet.Cost;
                walet += result.TotalReward;
            }

            tbResult.Text = sb.ToString();
        }

        public void TestOldResults()
        {
            var history = new int[][]
            {
                new int[] { 1, 4, 5, 6, 7, 8, 9, 11, 14, 16, 17, 19, 21, 23, 25 },
                new int[] { 1, 3, 4, 5, 6, 7, 12, 16, 17, 18, 20, 21, 22, 23, 24 },
                new int[] { 1, 2, 4, 6, 7, 10, 11, 12, 14, 18, 19, 20, 21, 23, 25 },
                new int[] { 1, 2, 6, 7, 9, 10, 11, 13, 14,  15, 17, 18, 19, 20, 23 },
                new int[] { 25  ,21 ,9  ,17 ,8  ,13 ,3  ,24 ,15 ,1  ,22 ,19 ,11 ,14 ,5  },
                new int[] { 21  ,6  ,3  ,2  ,24 ,11 ,12 ,25 ,23 ,8  ,18 ,15 ,13 ,1  ,17 },
                new int[] { 7   ,2  ,13 ,18 ,15 ,1  ,22 ,20 ,24 ,5  ,10 ,19 ,4  ,8  ,17 },
                new int[] { 9   ,15 ,13 ,6  ,7  ,24 ,10 ,25 ,14 ,22 ,3  ,2  ,16 ,17 ,21 },
                new int[] { 20  ,10 ,3  ,13 ,18 ,5  ,19 ,1  ,11 ,16 ,24 ,6  ,2  ,23 ,21 },
                new int[] { 13  ,16 ,10 ,18 ,21 ,19 ,5  ,11 ,3  ,4  ,24 ,8  ,25 ,22 ,7  },
                new int[] { 10  ,8  ,4  ,19 ,17 ,16 ,22 ,13 ,25 ,18 ,2  ,24 ,7  ,9  ,23 },
                new int[] { 15  ,19 ,20 ,18 ,5  ,25 ,1  ,12 ,16 ,3  ,23 ,21 ,10 ,24 ,9  },
                new int[] { 19  ,6  ,5  ,7  ,24 ,11 ,18 ,9  ,14 ,3  ,15 ,25 ,17 ,10 ,16 },
                new int[] { 20  ,9  ,3  ,2  ,13 ,12 ,16 ,14 ,23 ,21 ,15 ,4  ,19 ,7  ,5  },
                new int[] { 23  ,5  ,4  ,11 ,2  ,18 ,25 ,22 ,15 ,20 ,10 ,12 ,3  ,24 ,8  },
                new int[] { 8   ,7  ,14 ,20 ,21 ,2  ,5  ,16 ,10 ,1  ,15 ,6  ,13 ,12 ,22 },
                new int[] { 5   ,1  ,23 ,7  ,24 ,10 ,17 ,18 ,12 ,22 ,13 ,20 ,19 ,4  ,15 },
                new int[] { 11  ,12 ,2  ,21 ,5  ,16 ,25 ,15 ,20 ,8  ,24 ,23 ,19 ,14 ,1  },
                new int[] { 23  ,9  ,3  ,5  ,7  ,21 ,18 ,6  ,20 ,17 ,19 ,11 ,4  ,24 ,15 },
                new int[] { 17  ,13 ,10 ,20 ,22 ,5  ,9  ,19 ,15 ,1  ,24 ,25 ,14 ,8  ,11 },
                new int[] { 16  ,18 ,24 ,1  ,15 ,22 ,10 ,23 ,2  ,20 ,4  ,5  ,14 ,9  ,6  },
                new int[] { 21  ,2  ,14 ,15 ,16 ,19 ,20 ,25 ,4  ,5  ,7  ,11 ,9  ,1  ,23 },
                new int[] { 24  ,7  ,13 ,5  ,1  ,25 ,3  ,8  ,17 ,18 ,20 ,9  ,19 ,14 ,2  },
                new int[] { 16  ,24 ,5  ,7  ,11 ,18 ,13 ,22 ,12 ,8  ,3  ,20 ,15 ,19 ,25 },
            };

            var lotoFacil = new GameLotoFacil();
            var count = new int[25];

            var addCount = new Action<int[]>(delegate (int[] nums)
            {
                for (var n = 0; n < 25; n++)
                {
                    var has = nums.Contains(n + 1);
                    if (has)
                    {
                        count[n]++;
                    }
                    else
                    {
                        count[n]--;
                    }
                }
            });


            for (var i = 0; i < history.Length; i++)
            {

            }
        }
    }
}