namespace Runner.WebUI.Helpers
{
    public class ElementClass
    {
        private List<string> _cls;

        public ElementClass(params string?[] cls)
        {
            _cls = cls
                .Where(c => !string.IsNullOrEmpty(c))
                .Select(c => c!)
                .ToList();
        }

        public ElementClass AddIfIsNotNullOrEmpty(string? cls)
        {
            if (!string.IsNullOrEmpty(cls))
            {
                _cls.Add(cls);
            }
            return this;
        }

        public ElementClass AddIfStringIsNotNullOrEmpty(string? str, string cls)
        {
            if (!string.IsNullOrEmpty(str))
            {
                _cls.Add(cls);
            }
            return this;
        }

        public ElementClass AddIf(bool condition, string cls)
        {
            if (condition)
            {
                _cls.Add(cls);
            }
            return this;
        }

        public ElementClass AddIfEnum<T>(T type, T value, string cls) where T : Enum
        {
            if (type.Equals(value))
            {
                _cls.Add(cls);
            }
            return this;
        }

        public override string ToString()
        {
            return string.Join(' ', _cls);
        }
    }
}
