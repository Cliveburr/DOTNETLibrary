using System.Diagnostics.CodeAnalysis;

namespace Runner.Business.Datas2.Model
{
    public class DataHandlerItem : DataProperty
    {
        public bool AllowDelete { get; set; } = true;
        public bool AllowEdit { get; set; } = true;
        
        private DataHandlerItem()
        {
        }

        [SetsRequiredMembers]
        public DataHandlerItem(DataProperty dataProperty)
        {
            Name = dataProperty.Name;
            Type = dataProperty.Type;
            IsRequired = dataProperty.IsRequired;
            Value = dataProperty.Value;
        }

        public DataHandlerItem Clone()
        {
            return new DataHandlerItem
            {
                Name = Name,
                Type = Type,
                IsRequired = IsRequired,
                Value = Value?.Clone(),
                AllowDelete = AllowDelete,
                AllowEdit = AllowEdit
            };
        }

        public DataProperty ToDataProperty()
        {
            return new DataProperty
            {
                Name = Name,
                Type = Type,
                IsRequired = IsRequired,
                Value = Value
            };
        }
    }
}
