using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Knot.Business
{
    public class FindOptions
    {
        public bool LoadProperties { get; set; }
        public int? ChildsDepth { get; set; }
        public bool LoadChildsProperties { get; set; }
        public int? ParentDepth { get; set; }
        public bool LoadParentProperties { get; set; }
    }

    internal class FindOptionsTranslate
    {
        public bool LoadProperties { get; set; }
        public bool WithParentDepth { get; set; }
        public bool FullParentDepth { get; set; }
        public int ParentDepth { get; set; }
        public bool LoadParentProperties { get; set; }
        public bool WithChildsDepth { get; set; }
        public bool FullChildsDepth { get; set; }
        public int ChildsDepth { get; set; }
        public bool LoadChildsProperties { get; set; }
    }
}