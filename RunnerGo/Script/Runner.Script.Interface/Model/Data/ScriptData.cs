namespace Runner.Script.Interface.Model.Data
{
    public class ScriptData
    {
        private List<ScriptDataState> _states;

        public ScriptData(List<ScriptDataProperty>? properties)
        {
            _states = properties?
                .Select(p => new ScriptDataState { Property = p, State = ScriptDataStateType.Pristine })
                .ToList() ?? new List<ScriptDataState>();
        }

        private void Set(string name, ScriptDataTypeEnum type, object value)
        {
            var state = _states
                .FirstOrDefault(s => s.Property.Name == name);
            if (state is null)
            {
                _states.Add(new ScriptDataState
                {
                    Property = new ScriptDataProperty { Name = name, Type = type, Value = value },
                    State = ScriptDataStateType.Add
                });
            }
            else
            {
                state.Property.Type = type;
                state.Property.Value = value;

                if (state.State == ScriptDataStateType.Pristine)
                {
                    state.State = ScriptDataStateType.Modified;
                }
            }
        }

        public IReadOnlyList<ScriptDataState> GetStates()
        {
            return _states.AsReadOnly();
        }

        public ScriptData SetString(string name, string value)
        {
            Set(name, ScriptDataTypeEnum.String, value);
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
