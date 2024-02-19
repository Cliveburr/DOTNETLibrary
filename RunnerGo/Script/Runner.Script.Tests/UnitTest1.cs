using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using Runner.Script.Hosting.Scripts;
using Test = Microsoft.VisualStudio.TestTools.UnitTesting.Assert;
using Runner.Business.Datas.Model;

namespace Runner.Script.Tests
{
    public class InputData
    {
        [Required]
        [DefaultValue("test nome")]
        public required string MyName { get; set; }

        public required InputDataCompany Company { get; set; }
    }

    public class InputDataCompany
    {
        [Required]
        public required string Name { get; set; }
    }


    [TestClass]
    public class ScriptTests
    {
        [TestMethod]
        public void TestMapTypeToProperties()
        {
            var props = MapTypeToProperties.Map(typeof(InputData));

            Test.IsNotNull(props);
            Test.AreEqual(props.Count, 2);

            Test.AreEqual(props[0].Name, "MyName");
            Test.AreEqual(props[0].Type, DataTypeEnum.String);
            Test.AreEqual(props[0].Value?.StringValue, "test nome");

            Test.AreEqual(props[1].Value?.DataExpand?[0].Name, "Name");
            Test.AreEqual(props[1].Value?.DataExpand?[0].Type, DataTypeEnum.String);
        }
    }
}