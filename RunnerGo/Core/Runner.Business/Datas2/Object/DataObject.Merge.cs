using Runner.Business.Datas2.Model;
using Runner.Business.Datas2.PropertyHandler;

namespace Runner.Business.Datas2.Object
{
    public partial class DataObject
    {
		public DataObject Merge(List<DataProperty>? datas)
		{
			Merge(datas?.Select(d => new DataHandlerItem(d)).ToList());
            return this;
		}

		public DataObject Merge(List<DataHandlerItem>? datas)
		{
			if (datas is null)
			{
				return this;
            }
			foreach (var data in datas)
			{
				var has = _datas
                    .FirstOrDefault(d => d.Name == data.Name);
				if (has is null)
				{
                    datas.Add(data);
                }
                else
				{
                    var handler = PropertyHandlerSelector.Get(data.Type);
                    handler.Merge(has, data);
                }
			}
			return this;
		}
	}
}
