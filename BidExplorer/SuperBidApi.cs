using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace BidExplorer
{
    public class SuperBidApi
    {
        public string ClientId { get; set; } = "dzqC3VodSoXukD45BQKg3NQU6-faststore";


        private readonly string _urlOrderByPublishedAtDesc = @"https://api.sbwebservices.net/offer-query/seo/offers/?portalId=[2,15]&requestOrigin=marketplace&locale=pt_BR&timeZoneId=America%2FSao_Paulo&searchType=opened&urlSeo=https://www.superbid.net/categorias/industrial-maquinas-equipamentos&pageNumber={0}&pageSize={1}&orderBy=publishedAt:desc";
        //private readonly string _urlOrderByPriceAsc = @"https://api.sbwebservices.net/offer-query/seo/offers/?portalId=[2,15]&requestOrigin=marketplace&locale=pt_BR&timeZoneId=America%2FSao_Paulo&searchType=opened&urlSeo=https://www.superbid.net/categorias/industrial-maquinas-equipamentos&pageNumber={0}&pageSize={1}&orderBy=price:asc";
        private readonly int _maxPageSize = 1000;

        private class SuperBidOfferQueryResult
        {
            public int Total { get; set; }
            public SuperBidOfferItemQueryResult[] Offers { get; set; }
        }

        private class SuperBidOfferItemQueryResult
        {
            public long Id { get; set; }
            public float Price { get; set; }
            public DateTime PublishedAt { get; set; }
            public SuperBidOfferItemOfferDescriptionQueryResult OfferDescription { get; set; }
            public SuperBidOfferItemProductQueryResult Product { get; set; }

            public override string ToString()
            {
                return $"Id: {Id}, PublishedAt: {PublishedAt.ToLocalTime()}, Price: {Price} - {OfferDescription.OfferDescription}";
            }
        }

        private class SuperBidOfferItemOfferDescriptionQueryResult
        {
            public string OfferDescription { get; set; }
        }

        private class SuperBidOfferItemProductQueryResult
        {
            public string ShortDesc { get; set; }
        }

        private async Task<SuperBidOfferQueryResult> Request(string url)
        {
            var handler = new HttpClientHandler()
            {
                AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate
            };

            using (var client = new HttpClient(handler))
            {
                client.DefaultRequestHeaders.Add("accept", "application/json, application/hal+json");
                client.DefaultRequestHeaders.Add("accept-encoding", "gzip, deflate, br");
                client.DefaultRequestHeaders.Add("accept-language", "pt-BR,pt;q=0.9,en-US;q=0.8,en;q=0.7");
                client.DefaultRequestHeaders.Add("cache-control", "no-cache");
                client.DefaultRequestHeaders.Add("client_id", ClientId);
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

                var result = JsonConvert.DeserializeObject<SuperBidOfferQueryResult>(content);

                if (result == null)
                {
                    throw new NullReferenceException("SuperBidOfferQueryResult parse null from content: " + content);
                }

                return result;
            }
        }

        public async Task<(int, Offer)> GetTotalAndLastPusblished()
        {
            var urlRead = string.Format(_urlOrderByPublishedAtDesc, "1", "1");
            var readReadResult = await Request(urlRead);

            return (readReadResult.Total, ParseJson(readReadResult.Offers[0]));
        }

        public async Task<List<Offer>> GetOffers(int page, int size)
        {
            var urlRead = string.Format(_urlOrderByPublishedAtDesc, page.ToString(), size.ToString());
            var readReadResult = await Request(urlRead);

            return readReadResult.Offers
                .Select(ParseJson)
                .ToList();
        }

        public async Task<List<Offer>> GetAllOffers()
        {
            var urlReadFirst = string.Format(_urlOrderByPublishedAtDesc, "1", _maxPageSize.ToString());
            var readFirstResult = await Request(urlReadFirst);
            
            var missing = readFirstResult.Total - readFirstResult.Offers.Length;
            var page = 1;
            var result = readFirstResult.Offers
                .Select(ParseJson)
                .ToList();

            while (missing > 0)
            {
                var pageSize = Math.Min(missing, _maxPageSize);
                page++;

                var urlReadMore = string.Format(_urlOrderByPublishedAtDesc, page.ToString(), pageSize.ToString());
                var readMoreResult = await Request(urlReadMore);

                result.AddRange(readMoreResult.Offers
                    .Select(ParseJson));

                missing -= readMoreResult.Offers.Length;
            }

            return result;
        }

        private Offer ParseJson(SuperBidOfferItemQueryResult item)
        {
            return new Offer
            {
                Id = item.Id,
                Description = item.Product.ShortDesc,
                Price = item.Price,
                PublishedAt = item.PublishedAt,
            };
        }
    }
}
