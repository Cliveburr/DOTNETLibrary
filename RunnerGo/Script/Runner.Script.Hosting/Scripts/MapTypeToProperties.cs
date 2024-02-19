using Runner.Business.Datas.Model;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Reflection;

namespace Runner.Script.Hosting.Scripts
{
    public static class MapTypeToProperties
    {
        public static List<DataProperty>? Map(Type? type)
        {
            if (type is null)
            {
                return null;
            }

            var ret = new List<DataProperty>();
            var properties = type.GetProperties();
            foreach (var prop in properties)
            {
                var isRequired = prop.GetCustomAttribute<RequiredAttribute>() is not null;
                var valueAttr = prop.GetCustomAttribute<DefaultValueAttribute>();

                if (prop.PropertyType.Equals(typeof(string)))
                {
                    var defaultValue = valueAttr?.Value as string;

                    ret.Add(new DataProperty
                    {
                        Name = prop.Name,
                        Type = DataTypeEnum.String,
                        IsRequired = isRequired,
                        Value = defaultValue is null ?
                            null :
                            new DataValue { StringValue = defaultValue }
                    });
                }
                else if (typeof(IEnumerable<string>).IsAssignableFrom(prop.PropertyType))
                {
                    var defaultValue = valueAttr?.Value as IEnumerable<string>;

                    ret.Add(new DataProperty
                    {
                        Name = prop.Name,
                        Type = DataTypeEnum.StringList,
                        IsRequired = isRequired,
                        Value = defaultValue is null ?
                            null :
                            new DataValue { StringListValue = defaultValue?.ToList() }
                    });
                }
                else if (prop.PropertyType.IsClass)
                {
                    var defaultValue = Map(prop.PropertyType);

                    ret.Add(new DataProperty
                    {
                        Name = prop.Name,
                        Type = DataTypeEnum.Data,
                        IsRequired = isRequired,
                        Value = defaultValue is null ?
                            null :
                            new DataValue { DataExpand = defaultValue.Select(d => new DataHandlerItem(d)).ToList() }
                    });
                }
            }

            return ret;
        }
    }
}
