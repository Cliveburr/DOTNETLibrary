using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RefineryBoard.OLX.Data
{
    [Serializable]
    public class OfferData
    {
        public string Price { get; set; }
        public string Category { get; set; }
        public string Region { get; set; }
        public string Title { get; set; }
        public string Href { get; set; }
        public string Code { get; set; }
        public Status Status { get; set; }
        public string Weight { get; set; }
    }

    [Serializable]
    public enum Status
    {
        Analyze = 0,
        Interesting = 1,
        Completed = 2,
        Rejected = 3,
        Deleted = 4
    }
}