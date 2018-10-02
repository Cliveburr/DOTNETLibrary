using RefineryBoard.Activity;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RefineryBoard.OLX.Init
{
    public class OLXModel : ActivityModelBase
    {
        public ObservableCollection<Data.OfferData> Offers { get; set; }
        public Data.OfferData Selected { get; set; }

        public OLXModel()
        {
            Offers = new ObservableCollection<Data.OfferData>();
        }
    }
}