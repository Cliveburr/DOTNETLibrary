using Runner.Business.Datas2.Handler;
using Runner.Business.Datas2.Model;
using Test = Microsoft.VisualStudio.TestTools.UnitTesting.Assert;

namespace Runner.Business.Tests.Data
{
    [TestClass]
    public class StringTest
    {
        [TestMethod]
        public void Normal()
        {
            var obj = new DataObject
            {
                Properties = new List<DataItem>
                {
                    new DataItem { Name = "test1", Type = DataTypeEnum.String,
                        Value = new DataValue { StringValue = "um valor" } },
                    new DataItem { Name = "test2", Type = DataTypeEnum.String,
                        Value = new DataValue { StringValue = "outro valor" } },
                }
            };

            var handler = new DataHandler(obj);
            var validated = handler.Validate();

            Test.IsFalse(validated.Any());
        }

        [TestMethod]
        public void MissRequired()
        {
            var obj = new DataObject
            {
                Properties = new List<DataItem>
                {
                    new DataItem { Name = "test1", Type = DataTypeEnum.String, IsRequired = true,
                        Value = new DataValue {  } },
                    new DataItem { Name = "test2", Type = DataTypeEnum.String,
                        Value = new DataValue { StringValue = "outro valor" } },
                }
            };

            var handler = new DataHandler(obj);
            var validated = handler.Validate();

            Test.IsTrue(validated.Count() == 1);
        }

        [TestMethod]
        public void RequiredIsEmpty()
        {
            var obj = new DataObject
            {
                Properties = new List<DataItem>
                {
                    new DataItem { Name = "test1", Type = DataTypeEnum.String, IsRequired = true,
                        Value = new DataValue { StringValue = string.Empty } },
                    new DataItem { Name = "test2", Type = DataTypeEnum.String,
                        Value = new DataValue { StringValue = "outro valor" } },
                }
            };

            var handler = new DataHandler(obj);
            var validated = handler.Validate();

            Test.IsTrue(validated.Count() == 1);
        }

        [TestMethod]
        public void ApplyDefaultOnEmpty()
        {
            var prop = new DataItem
            {
                Name = "test1",
                Type = DataTypeEnum.String,
                IsRequired = true,
                Value = new DataValue { },
                Default = new DataValue { StringValue = "some value" }
            };

            var obj = new DataObject
            {
                Properties = [prop]
            };

            var handler = new DataHandler(obj);
            handler.ApplyDefaults();
            var validated = handler.Validate();

            Test.IsFalse(validated.Any());
            Test.AreEqual(prop.Value.StringValue, prop.Default.StringValue);
        }

        [TestMethod]
        public void ApplyDefaultOnValueNull()
        {
            var prop = new DataItem
            {
                Name = "test1",
                Type = DataTypeEnum.String,
                IsRequired = true,
                Value = null,
                Default = new DataValue { StringValue = "some value" }
            };

            var obj = new DataObject
            {
                Properties = [prop]
            };

            var handler = new DataHandler(obj);
            handler.ApplyDefaults();
            var validated = handler.Validate();

            Test.IsFalse(validated.Any());
            Test.AreEqual(prop.Value?.StringValue, prop.Default.StringValue);
        }

        [TestMethod]
        public void SimpleMerge()
        {
            var obj = new DataObject
            {
                Properties = [new DataItem
                {
                    Name = "test1",
                    Type = DataTypeEnum.String,
                    Value = new DataValue { StringValue = "nop" }
                }]
            };

            var fromObj = new DataObject
            {
                Properties = [new DataItem
                {
                    Name = "test1",
                    Type = DataTypeEnum.String,
                    Value = new DataValue { StringValue = "merged" }
                }]
            };

            var handler = new DataHandler(obj);
            handler.Merge(fromObj);

            var value = handler.ReadString("test1");
            Test.AreEqual(value, "merged");
        }

        [TestMethod]
        public void PreserveMerge()
        {
            var obj = new DataObject
            {
                Properties = [new DataItem
                {
                    Name = "test1",
                    Type = DataTypeEnum.String,
                    Value = new DataValue { StringValue = "preserved" }
                }]
            };

            var fromObj = new DataObject
            {
                Properties = [new DataItem
                {
                    Name = "test1",
                    Type = DataTypeEnum.String,
                    Value = null
                }]
            };

            var handler = new DataHandler(obj);
            handler.Merge(fromObj);

            var value = handler.ReadString("test1");
            Test.AreEqual(value, "preserved");
        }
    }
}
