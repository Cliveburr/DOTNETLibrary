using Runner.Business.Datas.Model;
using Runner.Business.Datas.PropertyHandler;

namespace Runner.Business.Datas.Object
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
			var toAdd = new List<DataHandlerItem>();
			foreach (var data in datas)
			{
				var has = Properties
                    .FirstOrDefault(d => d.Name == data.Name);
				if (has is null)
				{
                    toAdd.Add(data);
                }
                else
				{
                    var handler = PropertyHandlerSelector.Get(data.Type);
                    handler.Merge(has, data);
                }
			}
			Properties.AddRange(toAdd);
			return this;
		}
	}
}
