using Runner.Business.DataStruct.Validator;
using Runner.Business.Entities.Nodes.Types;
using Runner.Business.Entities.Nodes.Types.DataStruct.Validator;
using System.Collections.ObjectModel;

namespace Runner.Business.DataNode.Merges
{
    public class MergeData
    {
        public IReadOnlyList<DataProperty> Properties { get; private set; }

        private MergeData()
        {
            Properties = new ReadOnlyCollection<DataProperty>([]);
        }

        public static MergeData From(List<DataProperty> datas)
        {
            var merged = new MergeData();
            merged.ApplyData(datas);
            return merged;
        }

        public void ApplyType(List<DataTypeProperty> types)
        {
            var list = Properties.ToList();

            foreach (var type in types)
            {
                var has = list
                    .FirstOrDefault(d => d.Name == type.Name);
                if (has is null)
                {
                    var newMergedDataValue = new DataProperty
                    {
                        Name = type.Name,
                        Value = type.Default,
                        Type = type.Type
                    };
                    list.Add(newMergedDataValue);
                }
                else
                {
                    has.Type = type.Type;

                    var validation = DataValidator.Validate(type, has.Value);
                    if (validation.Any())
                    {
                        has.Value = type.Default; //TODO: try conversion
                    }
                }
            }

            Properties = new ReadOnlyCollection<DataProperty>(list);
        }

        public void ApplyData(List<DataProperty> datas)
        {
            var list = Properties.ToList();

            foreach (var data in datas)
            {
                var has = list
                    .FirstOrDefault(d => d.Name == data.Name);
                if (has is null)
                {
                    list.Add(data);
                }
                else
                {
                    has.Type = data.Type;
                    has.Value = data.Value;
                }
            }

            Properties = new ReadOnlyCollection<DataProperty>(list);
        }
    }
}
