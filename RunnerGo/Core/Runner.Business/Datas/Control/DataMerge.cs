using Runner.Business.Datas.Model;
using Runner.Business.Datas.Validator;

namespace Runner.Business.Datas.Control
{
    public class DataMerge
    {
        protected List<DataFullProperty> _datas;

        public DataMerge()
        {
            _datas = new List<DataFullProperty>();
        }

        public DataMerge ApplyDataProperty(List<DataProperty>? dataProperties)
        {
            if (dataProperties is null)
            {
                return this;
            }
            foreach (var dataProperty in dataProperties)
            {
                var has = _datas
                    .FirstOrDefault(d => d.Name == dataProperty.Name);
                if (has is null)
                {
                    _datas.Add(new DataFullProperty(dataProperty));
                }
                else
                {
                    has.Type = dataProperty.Type;
                    has.Value = dataProperty.Value;
                }
            }
            return this;
        }

        public DataMerge ApplyDataTypeProperty(List<DataTypeProperty>? dataTypeProperties)
        {
            if (dataTypeProperties is null)
            {
                return this;
            }
            foreach (var dataTypeProperty in dataTypeProperties)
            {
                var has = _datas
                    .FirstOrDefault(d => d.Name == dataTypeProperty.Name);
                if (has is null)
                {
                    var dataFullProperty = new DataFullProperty(dataTypeProperty);
                    dataFullProperty.Value = dataTypeProperty.Default;
                    _datas.Add(dataFullProperty);
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
            return this;
        }

        public DataMerge ApplyDataFullProperty(List<DataFullProperty>? dataFullProperties)
        {
            if (dataFullProperties is null)
            {
                return this;
            }
            foreach (var dataFullProperty in dataFullProperties)
            {
                var has = _datas
                    .FirstOrDefault(d => d.Name == dataFullProperty.Name);
                if (has is null)
                {
                    _datas.Add(dataFullProperty);
                }
                else
                {
                    has.Type = dataFullProperty.Type;
                    has.Default = dataFullProperty.Default;
                    has.IsRequired = dataFullProperty.IsRequired;

                    var validation = DataValidator.Validate(dataFullProperty.ToDataTypeProperty(), has.Value);
                    if (validation.Any())
                    {
                        has.Value = dataFullProperty.Default; //TODO: try conversion
                    }
                }
            }
            return this;
        }

        public List<DataProperty> ToDataProperty()
        {
            return _datas
                .Select(d => d.ToDataProperty())
                .ToList();
        }

        public List<DataTypeProperty> ToDataTypeProperty()
        {
            return _datas
                .Select(d => d.ToDataTypeProperty())
                .ToList();
        }

        public List<DataFullProperty> ToDataFullProperty()
        {
            return _datas;
        }
    }
}
