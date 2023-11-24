using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Runner.Business.Data.Types
{
    public class DataTypeStruct
    {
        public required List<DataTypeProperty> Properties { get; set; }
    }
}
