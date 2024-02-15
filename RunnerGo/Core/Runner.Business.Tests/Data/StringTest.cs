using Runner.Business.Datas2.Model;
using Runner.Business.Datas2.Object;
using Test = Microsoft.VisualStudio.TestTools.UnitTesting.Assert;

namespace Runner.Business.Tests.Data
{
    [TestClass]
    public class StringTest
    {
        [TestMethod]
        public void Normal()
        {
            var obj = new DataObject([
                    new DataProperty { Name = "test1", Type = DataTypeEnum.String,
                        Value = new DataValue { StringValue = "um valor" } },
                    new DataProperty { Name = "test2", Type = DataTypeEnum.String,
                        Value = new DataValue { StringValue = "outro valor" } }
                ]);

            var validated = obj.Validate();

            Test.IsFalse(validated.Any());
        }

        [TestMethod]
        public void MissRequired()
        {
            var obj = new DataObject([
                    new DataProperty { Name = "test1", Type = DataTypeEnum.String, IsRequired = true,
                        Value = new DataValue {  } },
                    new DataProperty { Name = "test2", Type = DataTypeEnum.String,
                        Value = new DataValue { StringValue = "outro valor" } }
                ]);

            var validated = obj.Validate();

            Test.IsTrue(validated.Count() == 1);
        }

        [TestMethod]
        public void RequiredIsEmpty()
        {
            var obj = new DataObject([
                    new DataProperty { Name = "test1", Type = DataTypeEnum.String, IsRequired = true,
                        Value = new DataValue { StringValue = string.Empty } },
                    new DataProperty { Name = "test2", Type = DataTypeEnum.String,
                        Value = new DataValue { StringValue = "outro valor" } },
            ]);

            var validated = obj.Validate();

            Test.IsTrue(validated.Count() == 1);
        }

        [TestMethod]
        public void SimpleMerge()
        {
            var prop = new DataProperty
            {
                Name = "test1",
                Type = DataTypeEnum.String,
                Value = new DataValue { StringValue = "nop" }
            };

            var fromObj = new DataProperty
            {
                Name = "test1",
                Type = DataTypeEnum.String,
                Value = new DataValue { StringValue = "merged" }
            };

            var obj = new DataObject([prop]);
            obj.Merge([fromObj]);

            var value = obj.ReadString("test1");
            Test.AreEqual(value, "merged");
        }

        [TestMethod]
        public void PreserveMerge()
        {
            var prop = new DataProperty
            {
                Name = "test1",
                Type = DataTypeEnum.String,
                Value = new DataValue { StringValue = "preserved" }
            };

            var fromObj = new DataProperty
            {
                Name = "test1",
                Type = DataTypeEnum.String,
                Value = null
            };

            var obj = new DataObject([prop]);
            obj.Merge([fromObj]);

            var value = obj.ReadString("test1");
            Test.AreEqual(value, "preserved");
        }
    }
}
