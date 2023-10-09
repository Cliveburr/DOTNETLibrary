using HtmlAgilityPack;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Xml.Linq;
using System.Xml.XPath;

namespace BidExplorer
{
    public class Testing
    {

        public async void Run()
        {
            //var url = @"https://www.superbid.net/categorias/industrial-maquinas-equipamentos?searchType=opened&pageNumber=1&pageSize=60&orderBy=lotNumber:desc;subLotNumber:desc";
            var url = @"https://api.sbwebservices.net/offer-query/seo/offers/?portalId=[2,15]&requestOrigin=marketplace&locale=pt_BR&timeZoneId=America%2FSao_Paulo&searchType=opened&urlSeo=https://www.superbid.net/categorias/industrial-maquinas-equipamentos&pageNumber=1&pageSize=60&orderBy=lotNumber:desc;subLotNumber:desc";

            var handler = new HttpClientHandler()
            {
                AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate
            };

            using (var client = new HttpClient(handler))
            {
                //client.DefaultRequestHeaders.Add(":authority", "api.sbwebservices.net");
                //client.DefaultRequestHeaders.Add(":path", "/offer-query/seo/offers/?portalId=[2,15]&requestOrigin=marketplace&locale=pt_BR&timeZoneId=America%2FSao_Paulo&searchType=opened&urlSeo=https://www.superbid.net/categorias/industrial-maquinas-equipamentos&pageNumber=1&pageSize=60&orderBy=lotNumber:asc;subLotNumber:desc");
                //client.DefaultRequestHeaders.Add(":scheme", "https");
                client.DefaultRequestHeaders.Add("accept", "application/json, application/hal+json");
                client.DefaultRequestHeaders.Add("accept-encoding", "gzip, deflate, br");
                client.DefaultRequestHeaders.Add("accept-language", "pt-BR,pt;q=0.9,en-US;q=0.8,en;q=0.7");
                //client.DefaultRequestHeaders.Add("authorization", "");
                client.DefaultRequestHeaders.Add("cache-control", "no-cache");
                client.DefaultRequestHeaders.Add("client_id", "dzqC3VodSoXukD45BQKg3NQU6-faststore");
                client.DefaultRequestHeaders.Add("origin", "https://www.superbid.net");
                client.DefaultRequestHeaders.Add("pragma", "no-cache");
                client.DefaultRequestHeaders.Add("referer", "https://www.superbid.net/");
                client.DefaultRequestHeaders.Add("sec-ch-ua", "\".Not / A)Brand\";v=\"99\", \"Google Chrome\";v=\"103\", \"Chromium\";v=\"103\"");
                client.DefaultRequestHeaders.Add("sec-ch-ua-mobile", "?0");
                client.DefaultRequestHeaders.Add("sec-ch-ua-platform", "\"Windows\"");
                client.DefaultRequestHeaders.Add("sec-fetch-dest", "empty");
                client.DefaultRequestHeaders.Add("sec-fetch-mode", "cors");
                client.DefaultRequestHeaders.Add("sec-fetch-site", "cross-site");
                client.DefaultRequestHeaders.Add("suppress-authenticate", "true");
                client.DefaultRequestHeaders.Add("user-agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/103.0.0.0 Safari/537.36");

                var content = await client.GetStringAsync(url);

                var json = JsonConvert.DeserializeObject(content);

                //var site = XDocument.Parse(content);

                //var container = site.XPathSelectElements("//div");
            }

        
            var web = new HtmlWeb();
            var doc = await web.LoadFromWebAsync(url);

            var body = doc.DocumentNode
                .SelectSingleNode("//body");

            var divs = body
                .SelectNodes("//div")
                .ToArray();
                //.Descendants("div"); // "//div[@class=\"MuiGrid-root MuiGrid-container MuiGrid-spacing-xs-2 css-isbt42\"]");

            var a = 1;
        }
    }
}
