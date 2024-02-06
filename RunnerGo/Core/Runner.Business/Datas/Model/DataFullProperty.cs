using System.Diagnostics.CodeAnalysis;

namespace Runner.Business.Datas.Model
{
    public sealed class DataFullProperty
    {
        public required string Name { get; set;  }
        public required DataTypeEnum Type { get; set; }
        public object? Default { get; set; }
        public bool? IsRequired { get; set; }
        public object? Value { get; set; }

        public bool IsEditable { get => !IsRequired.HasValue; }

        public Action<DataFullProperty>? OnValueChange { get; set; }

        [SetsRequiredMembers]
        public DataFullProperty(DataProperty dataProperty)
        {
            Name = dataProperty.Name;
            Type = dataProperty.Type;
            Value = dataProperty.Value;
        }

        [SetsRequiredMembers]
        public DataFullProperty(DataTypeProperty dataTypeProperty)
        {
            Name = dataTypeProperty.Name;
            Type = dataTypeProperty.Type;
            Default = dataTypeProperty.Default;
            IsRequired = dataTypeProperty.IsRequired;
        }

        public DataProperty ToDataProperty()
        {
            return new DataProperty
            {
                Name = Name,
                Type = Type,
                Value = Value
            };
        }

        public DataTypeProperty ToDataTypeProperty()
        {
            return new DataTypeProperty
            {
                Name = Name,
                Type = Type,
                Default = Default,
                IsRequired = IsRequired ?? false
            };
        }
    }
}
