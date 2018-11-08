using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Nodus.Core.Application.Tag
{
    public interface ITag
    {
        ITag Parent { get; set; }
        List<ITag> Childs { get; set; }
        XmlNode Node { get; set; }
        void Execute(Context context);
    }
}