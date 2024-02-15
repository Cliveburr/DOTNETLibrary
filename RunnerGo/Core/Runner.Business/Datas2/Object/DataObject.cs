using Runner.Business.Datas2.Model;
using Runner.Business.Datas2.PropertyHandler;

namespace Runner.Business.Datas2.Object
{
    public partial class DataObject
    {
        private List<DataHandlerItem> _datas;
        private IDataResolveService? _service;

        public DataObject(List<DataProperty>? datas = null, IDataResolveService? service = null)
        {
            _datas = datas?.Select(d => new DataHandlerItem(d)).ToList() ?? [];
            _service = service;
        }

        public IEnumerable<ValidationError> Validate()
        {
            foreach (var item in _datas)
            {
                var handler = PropertyHandlerSelector.Get(item.Type);
                foreach (var error in handler.Validate(item))
                {
                    yield return error;
                }
            }
        }

        public async Task Resolve()
        {
            Assert.MustNotNull(_service, "Need IDataResolveService for resolve!");

            foreach (var data in _datas)
            {
                var handler = PropertyHandlerSelector.Get(data.Type);
                await handler.Resolve(this, data, _service);
            }
        }
        public List<DataHandlerItem> ToItems()
        {
            return _datas;
        }

        public List<DataProperty> ToDataProperty()
        {
            return _datas
                .Select(d => d.ToDataProperty())
                .ToList();
        }
    }
}
