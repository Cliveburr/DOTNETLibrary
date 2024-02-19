using Runner.Business.Datas.Model;
using Runner.Business.Datas.PropertyHandler;

namespace Runner.Business.Datas.Object
{
    public partial class DataObject
    {
        public List<DataHandlerItem> Properties { get; init; }
        public IDataResolveService? Service { get; set; }

        public DataObject()
        {
            Properties = [];
        }

        public DataObject(IDataResolveService service)
        {
            Properties = [];
            Service = service;
        }

        public DataObject(List<DataProperty>? datas = null, IDataResolveService? service = null)
        {
            Properties = datas?.Select(d => new DataHandlerItem(d)).ToList() ?? [];
            Service = service;
        }

        public DataObject(List<DataHandlerItem>? datas = null, IDataResolveService? service = null)
        {
            Properties = datas?.ToList() ?? [];
            Service = service;
        }

        public IEnumerable<ValidationError> Validate()
        {
            foreach (var item in Properties)
            {
                var handler = PropertyHandlerSelector.Get(item.Type);
                foreach (var error in handler.Validate(item))
                {
                    yield return error;
                }
            }
        }

        public async Task Resolve(bool isRecursive = true)
        {
            Assert.MustNotNull(Service, "Need IDataResolveService for resolve!");

            var cleanDataobjectDatas = new DataObject(Service);
            foreach (var data in Properties)
            {
                var handler = PropertyHandlerSelector.Get(data.Type);
                await handler.Resolve(cleanDataobjectDatas, data, Service, isRecursive);
            }

            var resolvedObj = new DataObject(Properties, Service);
            resolvedObj.Merge(cleanDataobjectDatas.Properties);

            Properties.Clear();
            Properties.AddRange(resolvedObj.Properties);
        }

        public List<DataProperty> ToDataProperty()
        {
            return Properties
                .Select(d => d.ToDataProperty())
                .ToList();
        }

        public DataObject Clone()
        {
            return new DataObject(Properties
                .Select(d => d.Clone())
                .ToList(), Service);
        }
    }
}
