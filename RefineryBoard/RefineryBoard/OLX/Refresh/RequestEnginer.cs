using RefineryBoard.OLX.Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace RefineryBoard.OLX.Refresh
{
    public class RequestControl
    {
        public bool Cancel { get; set; }
        public Action<List<OfferData>> OnData { get; set; }
        public Action<uint> OnPage { get; set; }

        public RequestControl(Action<List<OfferData>> onData)
        {
            OnData = onData;
        }
    }

    public static class RequestEnginer
    {
        public static uint Page { get; set; }

        private static Regex _hasMoreRegex = new Regex(@"0\s+de\s+0\s+resultados", RegexOptions.Multiline | RegexOptions.IgnoreCase);
        private static Regex _extractPos0Regex = new Regex(@"\<div\s+class\=\"".*section_OLXad-list.*\"".*\>", RegexOptions.Multiline | RegexOptions.IgnoreCase);

        private static Regex _extractPos1Regex = new Regex(@"\<li\s+class\=\""\s*item\s*\""", RegexOptions.Multiline | RegexOptions.IgnoreCase);
        private static Regex _extractPos2Regex = new Regex(@"data\-list_id\=\""(.*?)\""", RegexOptions.Multiline | RegexOptions.IgnoreCase);
        private static Regex _extractPos3Regex = new Regex(@"href\=\""(.*?)\""", RegexOptions.Multiline | RegexOptions.IgnoreCase);

        private static Regex _extractPos4Regex = new Regex(@"\<div\s+class\=\""\s*col\-2.*\""", RegexOptions.Multiline | RegexOptions.IgnoreCase);
        private static Regex _extractPos5Regex = new Regex(@"\<h2.*\>(.*?)\<\/h2\>", RegexOptions.Singleline);
        private static Regex _extractPos6Regex = new Regex(@"\<p.*\>(.*?)\<\/p\>.*\<p.*\>(.*?)\<\/p\>.*col\-3", RegexOptions.Singleline);
        private static Regex _extractPos7Regex = new Regex(@"OLXad\-list\-price.*\"".*\>(.*)\<\/p\>.*col\-4", RegexOptions.Singleline | RegexOptions.IgnoreCase);

        static RequestEnginer()
        {
            Page = 1;
        }

        public static void Get(RequestControl control)
        {
            var data = string.Empty;

            do
            {
                data = GetData(Page);

                var offers = ExtractOffers(data);

                Page++;

                control.OnPage(Page);
                control.OnData(offers);

            } while (!(IsFinish(data) || control.Cancel));
        }

        private static string GetData(uint page)
        {
            var url = page == 1 ?
                Program.Data.Content.OLXUrl + "?q=" + Program.Data.Content.OLXQuerie :
                Program.Data.Content.OLXUrl + "?o=" + page.ToString() + "&q=" + Program.Data.Content.OLXQuerie;

            var request = WebRequest.Create(url);

            using (var response = request.GetResponse())
            {
                using (var dataStream = response.GetResponseStream())
                {
                    using (var reader = new StreamReader(dataStream, Encoding.GetEncoding("ISO-8859-1")))
                    {
                        return reader.ReadToEnd();
                    }
                }
            }
        }

        private static bool IsFinish(string data)
        {
            return _hasMoreRegex.IsMatch(data);
        }

        private static List<OfferData> ExtractOffers(string data)
        {
            var tr = new List<OfferData>();

            var firstPoint = _extractPos0Regex.Match(data);
            if (firstPoint.Success)
            {
                var subData = data.Substring(firstPoint.Index + firstPoint.Groups[0].Length);

                //var itemPoint = _extractPos1Regex.Match(subData);
                var items = _extractPos1Regex.Split(subData);
                //while (itemPoint.Success)
                foreach (var item in items)
                {
                    //subData = subData.Substring(itemPoint.Index + itemPoint.Groups[0].Length);
                    //var itemData = subData.Substring(0, 10000);
                    var itemData = item;
                    var newItem = new OfferData();

                    var code = _extractPos2Regex.Match(itemData);
                    if (code.Success)
                    {
                        newItem.Code = code.Groups[1].Value;
                    }
                    else
                    {
                        continue;
                    }

                    var href = _extractPos3Regex.Match(itemData);
                    if (href.Success)
                    {
                        newItem.Href = href.Groups[1].Value;
                        var hasQueryString = newItem.Href.IndexOf('?');
                        if (hasQueryString > -1)
                        {
                            newItem.Href = newItem.Href.Substring(0, hasQueryString);
                        }
                    }

                    var column2 = _extractPos4Regex.Match(itemData);
                    if (column2.Success)
                    {
                        var title = _extractPos5Regex.Match(itemData);
                        if (title.Success)
                        {
                            newItem.Title = CleanText(title.Groups[1].Value);
                        }

                        var regionCategory = _extractPos6Regex.Match(itemData);
                        if (regionCategory.Success)
                        {
                            newItem.Region = CleanText(regionCategory.Groups[1].Value);
                            newItem.Category = CleanText(regionCategory.Groups[2].Value);
                        }
                    }

                    var price = _extractPos7Regex.Match(itemData);
                    if (price.Success)
                    {
                        newItem.Price = CleanText(price.Groups[1].Value);
                    }

                    tr.Add(newItem);
                }
            }

            return tr;
        }

        private static string CleanText(string text)
        {
            return text
                .Replace("\t", "")
                .Replace("\r", "")
                .Replace("\n", "")
                .Trim();
        }
    }
}