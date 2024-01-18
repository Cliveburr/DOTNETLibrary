using Runner.Business.DataAccess;
using Runner.Business.Security;

namespace Runner.Business.Services.NodeTypes
{
    public class AgentService : DataServiceBase
    {
        private readonly IdentityProvider _identityProvider;
        private readonly NodeService _nodeService;

        public AgentService(Database database, IdentityProvider identityProvider, NodeService nodeService)
            : base(database)
        {
            _identityProvider = identityProvider;
            _nodeService = nodeService;
        }
    }
}
