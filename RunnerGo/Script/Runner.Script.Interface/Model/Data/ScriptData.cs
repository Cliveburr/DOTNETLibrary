namespace Runner.Script.Interface.Model.Data
{
    public class ScriptData
    {
        private List<ScriptDataProperty>? _input;
        private List<ScriptDataProperty> _output;

        public ScriptData(List<ScriptDataProperty>? input)
        {
            _input = input;
            _output = new List<ScriptDataProperty>();
        }

        private void Set(string name, ScriptDataTypeEnum type, ScriptDataValue value)
        {
            var state = _output
                .FirstOrDefault(s => s.Name == name);
            if (state is null)
            {
                _output.Add(new ScriptDataProperty
                {
                    Name = name,
                    Type = type,
                    Value = value
                });
            }
            else
            {
                state.Type = type;
                state.Value = value;
            }
        }

        public IReadOnlyList<ScriptDataProperty> GetOutput()
        {
            return _output.AsReadOnly();
        }

        public ScriptData SetString(string name, string value)
        {
            Set(name, ScriptDataTypeEnum.String, new ScriptDataValue
            {
                StringValue = value
            });
            return this;
        }

        public T? ReadInput<T>()
        {
            if (_input is null)
            {
                return default;
            }

            var obj = Activator.CreateInstance<T>();
            if (obj is null)
            {
                return default;
            }

            PopulateObj(obj, _input);

            return obj;
        }

        private void PopulateObj(object obj, List<ScriptDataProperty> properties)
        {
            var objType = obj.GetType();
            foreach (var prop in properties)
            {
                if (prop.Value is null)
                {
                    continue;
                }

                var propertyInfo = objType.GetProperty(prop.Name);
                if (propertyInfo is null)
                {
                    continue;
                }

                switch (prop.Type)
                {
                    case ScriptDataTypeEnum.String:
                        propertyInfo.SetValue(obj, prop.Value.StringValue);
                        break;
                    case ScriptDataTypeEnum.StringList:
                        propertyInfo.SetValue(obj, prop.Value.StringListValue);
                        break;
                    case ScriptDataTypeEnum.Node:
                    case ScriptDataTypeEnum.Inherit:
                    case ScriptDataTypeEnum.ScriptVersion:
                        propertyInfo.SetValue(obj, prop.Value.NodePath);
                        break;
                    case ScriptDataTypeEnum.Data:
                        {
                            var propInst = Activator.CreateInstance(propertyInfo.PropertyType);
                            if (propInst is not null && prop.Value.DataExpand is not null)
                            {
                                propertyInfo.SetValue(obj, propInst);
                                PopulateObj(propInst, prop.Value.DataExpand);
                            }
                            break;
                        }
                }
            }
        }
    }
}
