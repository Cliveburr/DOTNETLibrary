namespace Runner.Script.Interface.Model.Data
{
    public class Data
    {
        private List<DataState> _states;

        public Data(List<DataProperty> properties)
        {
            _states = properties
                .Select(p => new DataState { Property = p, State = DataStateType.Pristine })
                .ToList();
        }

        private void Set(string name, DataTypeEnum type, object value)
        {
            var state = _states
                .FirstOrDefault(s => s.Property.Name == name);
            if (state is null)
            {
                _states.Add(new DataState
                {
                    Property = new DataProperty { Name = name, Type = type, Value = value },
                    State = DataStateType.Add
                });
            }
            else
            {
                state.Property.Type = type;
                state.Property.Value = value;

                if (state.State == DataStateType.Pristine || state.State == DataStateType.Deleted)
                {
                    state.State = DataStateType.Modified;
                }
            }
        }

        public Data Delete(string name)
        {
            var state = _states
                .FirstOrDefault(s => s.Property.Name == name);
            if (state is not null)
            {
                if (state.State == DataStateType.Add)
                {
                    _states.Remove(state);
                }
                else
                {
                    state.State = DataStateType.Deleted;
                }
            }
            return this;
        }

        public List<T> MapTo<T>(Func<DataState, T> selector)
        {
            return _states.Select(selector).ToList();
        }

        public Data SetString(string name, string value)
        {
            Set(name, DataTypeEnum.String, value);
            return this;
        }

        public string? GetString(string name)
        {
            var state = _states
                .FirstOrDefault(s => s.Property.Name == name);
            if (state is null)
            {
                return null;
            }
            else
            {
                return state.Property.Value as string;
            }
        }
    }
}
