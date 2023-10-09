using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BidExplorer
{
    public class Offer
    {
        public long Id { get; set; }
        public string Description { get; set; }
        public float Price { get; set; }
        public DateTime PublishedAt { get; set; }
    }
}
