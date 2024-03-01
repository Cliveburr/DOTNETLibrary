using Runner.Script.Interface.Model.Data;

namespace Build.Scripts.Tests
{
    public abstract class TestsBase
    {
        protected ScriptDataProperty StringProperty(string name, string value)
        {
            return new ScriptDataProperty
            {
                Name = name,
                Type = ScriptDataTypeEnum.String,
                IsRequired = false,
                Value = new ScriptDataValue
                {
                    StringValue = value
                }
            };
        }

        protected Task WriteLog(string message)
        {
            Console.WriteLine(message);
            return Task.CompletedTask;
        }
    }
}
