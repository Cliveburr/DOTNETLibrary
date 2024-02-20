using Microsoft.AspNetCore.Mvc.ViewFeatures;
using MongoDB.Bson;
using Runner.Business.Datas.Model;
using Runner.Business.Datas.Object;
using System.ComponentModel.DataAnnotations;
using Test = Microsoft.VisualStudio.TestTools.UnitTesting.Assert;

namespace Runner.Business.Tests.Data
{
    [TestClass]
    public class UseCases
    {
        private static ObjectId inputTypeId = ObjectId.GenerateNewId();
        private static ObjectId companyTypeId = ObjectId.GenerateNewId();
        private static ObjectId companyDataId = ObjectId.GenerateNewId();

        public class CompanyResolveService : IDataResolveService
        {
            public Task<List<DataProperty>?> ResolveDataProperties(ObjectId objectId)
            {
                if (objectId == inputTypeId) // dataType para ser input de uma action
                {
                    return Task.FromResult<List<DataProperty>?>([
                        new DataProperty { Name = "agentpool", Type = DataTypeEnum.String, IsRequired = true },
                        new DataProperty { Name = "company", Type = DataTypeEnum.Data, IsRequired = true, Value = new DataValue { ObjectIdValue = companyTypeId } }
                    ]);
                }
                else if (objectId == companyTypeId) // dataType que define as propriedades de uma company
                {
                    return Task.FromResult<List<DataProperty>?>([
                        new DataProperty { Name = "name", Type = DataTypeEnum.String, IsRequired = true },
                        new DataProperty { Name = "id", Type = DataTypeEnum.String }
                    ]);
                }
                else if (objectId == companyDataId)  // data que define uma company
                {
                    return Task.FromResult<List<DataProperty>?>([
                        new DataProperty { Name = "name", Type = DataTypeEnum.String, Value = new DataValue { StringValue = "company test" } }
                    ]);
                }

                return Task.FromResult<List<DataProperty>?>(null);
            }

            //public Task<string?> ResolveNodePath(ObjectId objectId)
            //{
            //    throw new NotImplementedException();
            //}

            public Task<List<DataProperty>?> ResolveScriptVersionInputProperties(ObjectId scriptId, string vesion)
            {
                throw new NotImplementedException();
            }
        }

        [TestMethod]
        public async Task DataType()
        {
            // data para uma action
            var data = new DataObject([
                new DataProperty { Name = "inputInherit", Type = DataTypeEnum.Inherit, Value = new DataValue { ObjectIdValue = inputTypeId } },
                new DataProperty { Name = "agentpool", Type = DataTypeEnum.String, Value = new DataValue { StringValue = "/some/path" } },
                new DataProperty { Name = "company", Type = DataTypeEnum.Data, Value = new DataValue { ObjectIdValue = companyDataId } }
            ], new CompanyResolveService());

            await data.Resolve();

            var validated = data.Validate();
            Test.IsFalse(validated.Any());

            var value = data.ReadString("company.name");
            Test.AreEqual(value, "company test");
        }
    }
}
