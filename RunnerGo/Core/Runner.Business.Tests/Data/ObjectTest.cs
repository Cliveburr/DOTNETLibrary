using MongoDB.Bson;
using Runner.Business.Datas2.Handler;
using Runner.Business.Datas2.Model;
using Test = Microsoft.VisualStudio.TestTools.UnitTesting.Assert;

namespace Runner.Business.Tests.Data
{
    [TestClass]
    public class ObjectTest
    {
        public class ObjectExpandService : IDataResolveService
        {
            public Task<DataObject> ExpandDataObject(ObjectId objectId)
            {
                return Task.FromResult(new DataObject
                {
                    Properties = [
                        new DataItem { Name = "second", Type = DataTypeEnum.String,
                            Value = new DataValue { StringValue = "this value" } }
                    ]
                });
            }

            public Task<string> ExpandNodePath(ObjectId objectId)
            {
                throw new NotImplementedException();
            }

            public Task<DataObject> ExpandScriptVersion(ObjectId scriptId, int vesion)
            {
                throw new NotImplementedException();
            }
        }

        [TestMethod]
        public async Task Expand()
        {
            var obj = new DataObject
            {
                Properties = new List<DataItem>
                {
                    new DataItem { Name = "first", Type = DataTypeEnum.Object,
                        Value = new DataValue { ObjectIdValue = ObjectId.GenerateNewId() } }
                }
            };

            var handler = new DataHandler(obj, new ObjectExpandService());
            await handler.Expand();

            var thisValue = handler.ReadString("first.second");
            Test.AreEqual(thisValue, "this value");
        }
    }
}
