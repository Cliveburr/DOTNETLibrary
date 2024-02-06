using Runner.Business.Datas.Model;
using Runner.Business.Datas.Validator;

namespace Runner.Business.Datas.Control
{
    public static class DataMerge
    {
        public static List<DataFullProperty> Merge(List<DataProperty>? dataProperties = null, List<DataTypeProperty>? dataTypeProperties = null, List<DataFullProperty>? dataFullProperties = null)
        {
            var tr = dataFullProperties?.ToList() ?? new List<DataFullProperty>();
            if (dataTypeProperties is not null)
            {
                ApplyDataTypeProperty(tr, dataTypeProperties);
            }
            if (dataProperties is not null)
            {
                ApplyDataProperty(tr, dataProperties);
            }
            return tr;
        }

        private static void ApplyDataProperty(List<DataFullProperty> dataFullProperties, List<DataProperty> dataProperties)
        {
            foreach (var dataProperty in dataProperties)
            {
                var has = dataFullProperties
                    .FirstOrDefault(d => d.Name == dataProperty.Name);
                if (has is null)
                {
                    dataFullProperties.Add(new DataFullProperty(dataProperty));
                }
                else
                {
                    has.Type = dataProperty.Type;
                    has.Value = dataProperty.Value;
                }
            }
        }

        private static void ApplyDataTypeProperty(List<DataFullProperty> dataFullProperties, List<DataTypeProperty> dataTypeProperties)
        {
            foreach (var dataTypeProperty in dataTypeProperties)
            {
                var has = dataFullProperties
                    .FirstOrDefault(d => d.Name == dataTypeProperty.Name);
                if (has is null)
                {
                    var dataFullProperty = new DataFullProperty(dataTypeProperty);
                    dataFullProperty.Value = dataTypeProperty.Default;
                    dataFullProperties.Add(dataFullProperty);
                }
                else
                {
                    has.Type = dataTypeProperty.Type;
                    has.Default = dataTypeProperty.Default;
                    has.IsRequired = dataTypeProperty.IsRequired;

                    var validation = DataValidator.Validate(dataTypeProperty, has.Value);
                    if (validation.Any())
                    {
                        has.Value = dataTypeProperty.Default; //TODO: try conversion
                    }
                }
            }
        }
    }
}
