using System.Diagnostics.CodeAnalysis;

namespace Runner.Business.Datas.Model
{
    public class DataHandlerItem : DataProperty
    {
        public bool AllowModify { get; set; } = true;
        
        public DataHandlerItem()
        {
        }

        [SetsRequiredMembers]
        public DataHandlerItem(DataProperty dataProperty, bool allowModify = true)
        {
            Name = dataProperty.Name;
            Type = dataProperty.Type;
            IsRequired = dataProperty.IsRequired;
            Value = dataProperty.Value;
            AllowModify = allowModify;
        }

        public DataHandlerItem Clone()
        {
            return new DataHandlerItem
            {
                Name = Name,
                Type = Type,
                IsRequired = IsRequired,
                Value = Value?.Clone(),
                AllowModify = AllowModify
            };
        }

        public DataProperty ToDataProperty()
        {
            return new DataProperty
            {
                Name = Name,
                Type = Type,
                IsRequired = IsRequired,
                Value = Value?.Clone()
            };
        }
    }
}
