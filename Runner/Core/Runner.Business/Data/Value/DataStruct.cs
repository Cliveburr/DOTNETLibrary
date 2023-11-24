using Runner.Business.Data.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Runner.Business.Data.Value
{
    public class DataStruct
    {
        public required List<DataProperty> Properties { get; set; }
    }
}
