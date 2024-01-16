using Runner.Business.DataAccess;

namespace Runner.Business.Services
{
    public class NodeService : DataServiceBase
    {
        public NodeService(Database database)
            : base(database)
        {
        }
    }
}
