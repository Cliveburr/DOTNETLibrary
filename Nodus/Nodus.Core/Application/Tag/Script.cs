using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Nodus.Core.Application.Tag
{
    public class Script : ITag
    {
        public ITag Parent { get; set; }
        public List<ITag> Childs { get; set; } = new List<ITag>();
        public string Name { get; set; }
        public XmlDocument Doc { get; set; }
        public XmlNode Node { get; set; }

        public Script(string name, XmlDocument doc)
        {
            Name = name;
            Doc = doc;
        }

        public void Execute(Context context)
        {
            throw new NotImplementedException();
        }
    }
}