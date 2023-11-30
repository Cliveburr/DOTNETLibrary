using Runner.Business.Data.Types;
using Runner.Business.Data.Value;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Runner.Business.Data
{
    public class DataWithType
    {
        public required DataProperty Data { get; set; }
        public required DataTypeProperty Type { get; set; }
    }

    public static class DataWithTypeApply
    {
        public static List<DataWithType> Mix(List<DataProperty> datas, List<DataTypeProperty> types)
        {
            var newData = new List<DataProperty>();
            var tr = new List<DataWithType>();
            foreach (var type in types)
            {
                var hasData = datas
                    .FirstOrDefault(d => d.Name == type.Name);
                if (hasData is null)
                {
                    var data = new DataProperty
                    {
                        Name = type.Name,
                        Value = type.Default
                    };
                    newData.Add(data);
                    tr.Add(new DataWithType { Data = data, Type = type });
                }
                else
                {
                    newData.Add(hasData);
                    tr.Add(new DataWithType { Data = hasData, Type = type });
                }
            }
            datas.Clear();
            datas.AddRange(newData);
            return tr;
        }
    }
}
