using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Runner.Communicator.Tests.Model
{
    public class OneModel
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? NullStr { get; set; }
        public TwoModel? TwoModel { get; set; }
        public TwoModel? NullTwoModel { get; set; }
    }

    public class TwoModel
    {
        public long Id { get; set; }
        public bool Active { get; set; }
        public required List<ThreeModel> ThreeModels { get; set; }
    }

    public class ThreeModel
    {
        public DateTime NowDateTime { get; set; }
    }
}
