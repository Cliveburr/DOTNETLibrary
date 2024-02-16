using MongoDB.Bson;
using Runner.Business.Datas.Model;
using Runner.Business.Datas.Object;
using Test = Microsoft.VisualStudio.TestTools.UnitTesting.Assert;

namespace Runner.Business.Tests.Data
{
    [TestClass]
    public class ObjectTest
    {
        public class ObjectExpandService : IDataResolveService
        {
            public Task<List<DataProperty>?> ResolveDataProperties(ObjectId objectId)
            {
                return Task.FromResult<List<DataProperty>?>([
                        new DataProperty
                        {
                            Name = "second",
                            Type = DataTypeEnum.String,
                            Value = new DataValue { StringValue = "this value" }
                        }
                    ]);
            }

            public Task<string?> ResolveNodePath(ObjectId objectId)
            {
                throw new NotImplementedException();
            }

            public Task<List<DataProperty>?> ResolveScriptVersionInputProperties(ObjectId scriptId, int vesion)
            {
                throw new NotImplementedException();
            }
        }

        [TestMethod]
        public async Task Resolve()
        {
            var obj = new DataObject([
                    new DataProperty { Name = "first", Type = DataTypeEnum.Data,
                        Value = new DataValue { ObjectIdValue = ObjectId.GenerateNewId() } }
                ], new ObjectExpandService());

            await obj.Resolve();

            var thisValue = obj.ReadString("first.second");
            Test.AreEqual(thisValue, "this value");
        }
    }
}
