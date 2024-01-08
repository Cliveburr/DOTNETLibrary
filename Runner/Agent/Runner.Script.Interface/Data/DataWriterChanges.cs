using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Runner.Script.Interface.Data
{
    public class DataWriterChanges
    {
        public required string Name { get; set; }
        public DataWriterCommand Command { get; set; }
        public DataTypeEnum Type { get; set; }
        public object? Value { get; set; }
    }
}
