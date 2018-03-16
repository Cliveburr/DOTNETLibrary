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
    }
}
