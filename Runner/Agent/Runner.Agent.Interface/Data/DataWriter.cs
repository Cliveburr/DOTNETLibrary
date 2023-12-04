using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Runner.Agent.Interface.Data
{
    public class DataWriterChanges
    {
        public required string Name { get; set; }
        public DataWriterCommand Command { get; set; }
        public DataTypeEnum Type { get; set; }
        public object? Value { get; set; }
    }

    public class DataWriter
    {
        public List<DataWriterChanges> Changes { get; private set; }

        public DataWriter()
        {
            Changes = new List<DataWriterChanges>();
        }

        private void Set(string name, DataTypeEnum type, object value)
        {
            var has = Changes
                .FirstOrDefault(c => c.Name == name);
            if (has is null)
            {
                Changes.Add(new DataWriterChanges
                {
                    Name = name,
                    Command = DataWriterCommand.Set,
                    Type = type,
                    Value = value
                });
            }
            else
            {
                has.Command = DataWriterCommand.Set;
                has.Type = type;
                has.Value = value;
            }
        }

        public DataWriter Delete(string name, string value)
        {
            var has = Changes
                .FirstOrDefault(c => c.Name == name);
            if (has is null)
            {
                Changes.Add(new DataWriterChanges
                {
                    Name = name,
                    Command = DataWriterCommand.Delete
                });
            }
            else
            {
                has.Command = DataWriterCommand.Delete;
                has.Value = null;
            }
            return this;
        }

        public DataWriter SetString(string name, string value)
        {
            Set(name, DataTypeEnum.String, value);
            return this;
        }
    }
}
