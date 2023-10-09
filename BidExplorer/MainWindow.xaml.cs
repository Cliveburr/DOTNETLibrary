using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace BidExplorer
{
    public partial class MainWindow : Window
    {
        private SuperBidApi _api;
        private DateTime _lastPublished;
        private long _lastPublishedId;
        private readonly string _dateTimeBrFormat = "dd/MM/yyyy HH:mm:ss";
        private Timer _timer;
        private int _timerInterval = 60000;
        private int _minPageSize = 60;

        public MainWindow()
        {
            InitializeComponent();

            //new Testing().Run();
            //new SuperBidApi().GetAllOffers();
            _api = new SuperBidApi();
            _timer = new Timer();
            _timer.Elapsed += _timer_Elapsed;
            _timer.Interval = _timerInterval;
            MainWindowStart();
        }

        private async void MainWindowStart()
        {
            WriteLog($"Iniciando em {DateTime.Now.ToString(_dateTimeBrFormat)}...");
            var (total, lastPublished) = await _api.GetTotalAndLastPusblished();
            _lastPublishedId = lastPublished.Id;
            _lastPublished = lastPublished.PublishedAt;
            //_lastPublishedId = 1234;
            //_lastPublished = DateTime.Parse("7/15/2022 0:00:00 PM");
            WriteLog($"Total de {total} ofertas e ultimo publicado em {_lastPublished.ToLocalTime().ToString(_dateTimeBrFormat)}");
            _timer.Start();
        }

        private void _timer_Elapsed(object? sender, ElapsedEventArgs e)
        {
            _timer.Stop();
            CheckNewOffers();
        }

        private void WriteLog(string text)
        {
            Dispatcher.Invoke(new Action(() =>
            {
                txBlock.Text = text + Environment.NewLine + txBlock.Text;
            }));
        }

        private async void CheckNewOffers()
        {
            var start = DateTime.Now;

            WriteLog($"Buscando novos items em {start.ToString(_dateTimeBrFormat)}..." + Environment.NewLine);

            var page = 1;
            var firstPage = true;
            var thisLastPublished = _lastPublished;
            var thisLastPublishedId = _lastPublishedId;

            while (true)
            {
                var items = await _api.GetOffers(page, _minPageSize);

                var newItems = items
                    .Where(i => i.PublishedAt > thisLastPublished && i.Id != thisLastPublishedId)
                    .ToList();

                if (!newItems.Any())
                {
                    WriteLog($"Busca finalizada em {(DateTime.Now - start).ToString("c")}");
                    _timer.Start();
                    return;
                }

                if (firstPage)
                {
                    firstPage = false;
                    _lastPublished = newItems[0].PublishedAt;
                    _lastPublishedId = newItems[0].Id;
                }

                var text = "";
                foreach (var newItem in newItems)
                {
                    text += $"Novo item {newItem.Id} - {newItem.PublishedAt.ToString(_dateTimeBrFormat)} - {newItem.Description}" + Environment.NewLine;
                }
                WriteLog(text);

                if (newItems.Count < items.Count)
                {
                    WriteLog($"Busca finalizada em {(DateTime.Now - start).ToString("c")}");
                    _timer.Start();
                    return;
                }

                page++;
            }
        }
    }
}
