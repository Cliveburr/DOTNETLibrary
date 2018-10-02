using RefineryBoard.OLX.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RefineryBoard.Data
{
    [Serializable()]
    public class ContentData
    {
        public List<OfferData> OLXOffers { get; set; }
        public string OLXUrl { get; set; }
        public string OLXQuerie { get; set; }

        public void CheckAndDefaultConfigs()
        {
            if (OLXOffers == null)
            {
                OLXOffers = new List<OfferData>();
            }
            if (string.IsNullOrEmpty(OLXUrl))
            {
                OLXUrl = "https://sp.olx.com.br/regiao-de-ribeirao-preto";
            }
            if (string.IsNullOrEmpty(OLXQuerie))
            {
                OLXQuerie = "ouro";
            }
        }
    }
}