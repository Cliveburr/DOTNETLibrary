
namespace Runner.Business.Datas.Control
{
    public class DataReader : DataMerge
    {
        public string? ReadString(string dataName)
        {
            return _datas
                .FirstOrDefault(d => d.Name == dataName &&
                    d.Type == Model.DataTypeEnum.String)?.Value as string;
        }

        public string? ReadNodePath(string dataName)
        {
            return _datas
                .FirstOrDefault(d => d.Name == dataName &&
                    d.Type == Model.DataTypeEnum.NodePath)?.Value as string;
        }

        public List<string>? ReadStringList(string dataName)
        {
            return _datas
                .FirstOrDefault(d => d.Name == dataName &&
                    d.Type == Model.DataTypeEnum.StringList)?.Value as List<string>;
        }
    }
}
