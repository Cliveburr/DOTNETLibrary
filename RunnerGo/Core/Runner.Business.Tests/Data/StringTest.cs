//using Runner.Business.Datas.Model;
//using Runner.Business.Datas.Validator;
//using Test = Microsoft.VisualStudio.TestTools.UnitTesting.Assert;

//namespace Runner.Business.Tests.Data
//{
//    [TestClass]
//    public class StringTest
//    {
//        [TestMethod]
//        public void Normal()
//        {
//            var type = new DataTypeStruct
//            {
//                Properties = new List<DataTypeProperty>
//                {
//                    new DataTypeProperty { Name = "test1", Type = DataTypeEnum.String },
//                    new DataTypeProperty { Name = "test2", Type = DataTypeEnum.String }
//                }
//            };
//            var data = new DataStruct
//            {
//                Properties = new List<DataProperty>
//                {
//                    new DataProperty { Name = "test1", Value = "um valor" },
//                    new DataProperty { Name = "test2", Value ="outro valor" }
//                }
//            };

//            var validated = DataValidator.Validate(data, type);

//            Test.IsFalse(validated.Any());
//        }

//        [TestMethod]
//        public void MissRequired()
//        {
//            var type = new DataTypeStruct
//            {
//                Properties = new List<DataTypeProperty>
//                {
//                    new DataTypeProperty { Name = "test1", Type = DataTypeEnum.String },
//                    new DataTypeProperty { Name = "test2", Type = DataTypeEnum.String, IsRequired = true }
//                }
//            };
//            var data = new DataStruct
//            {
//                Properties = new List<DataProperty>
//                {
//                    new DataProperty { Name = "test1", Value = "um valor" }
//                }
//            };

//            var validated = DataValidator.Validate(data, type);

//            Test.IsTrue(validated.Count() == 1);
//        }

//        [TestMethod]
//        public void RequiredIsEmpty()
//        {
//            var type = new DataTypeStruct
//            {
//                Properties = new List<DataTypeProperty>
//                {
//                    new DataTypeProperty { Name = "test1", Type = DataTypeEnum.String },
//                    new DataTypeProperty { Name = "test2", Type = DataTypeEnum.String, IsRequired = true }
//                }
//            };
//            var data = new DataStruct
//            {
//                Properties = new List<DataProperty>
//                {
//                    new DataProperty { Name = "test1", Value = "um valor" },
//                    new DataProperty { Name = "test2", Value = string.Empty }
//                }
//            };

//            var validated = DataValidator.Validate(data, type);

//            Test.IsTrue(validated.Count() == 1);
//        }
//    }
//}
