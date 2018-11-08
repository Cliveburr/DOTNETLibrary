using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Nodus.Core.Application.Tag
{
    public class Message : ITag
    {
        public ITag Parent { get; set; }
        public List<ITag> Childs { get; set; } = new List<ITag>();
        public XmlNode Node { get; set; }

        public void Execute(Context context)
        {
            var value = Node.Attributes.GetNamedItem("value");

            if (value != null)
            {
                context.Result.Messager += value.Value;
            }
        }
    }
}