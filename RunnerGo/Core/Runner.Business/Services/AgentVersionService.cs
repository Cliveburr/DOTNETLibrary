using MongoDB.Bson;
using MongoDB.Driver;
using Runner.Business.DataAccess;
using Runner.Business.Entities.AgentVersion;
using Runner.Business.Security;

namespace Runner.Business.Services
{
    public class AgentVersionService : DataServiceBase
    {
        private readonly IdentityProvider _identityProvider;

        public AgentVersionService(Database database, IdentityProvider identityProvider)
            : base(database)
        {
            _identityProvider = identityProvider;
        }

        public Task<List<AgentVersion>> Read()
        {
            return AgentVersion
                .ToListAsync();
        }

        public Task<AgentVersion> ReadLatest()
        {
            var versionSortByVersion = Builders<AgentVersion>.Sort
                .Descending(av => av.Version);

            return AgentVersion.Collection
                .Find(Builders<AgentVersion>.Filter.Empty)
                .Sort(versionSortByVersion)
                .FirstOrDefaultAsync();
        }

        public async Task Create(string fileName, byte[] fileContent)
        {
            Assert.MustNotNull(_identityProvider.User, "Need to be logged to upload new Agent Version!");

            //TODO: check if is admin

            var versionSortByVersion = Builders<AgentVersion>.Sort
                .Descending(av => av.Version);

            var lastVersion = await AgentVersion.Collection
                .Find(Builders<AgentVersion>.Filter.Empty)
                .Sort(versionSortByVersion)
                .FirstOrDefaultAsync();

            var nextVersion = lastVersion == null ?
                0 :
                lastVersion.Version + 1;

            var agentVersion = new AgentVersion
            {
                CreatedUtc = DateTime.UtcNow,
                Version = nextVersion,
                FileName = fileName,
                FileContent = fileContent
            };

            await AgentVersion
                .InsertAsync(agentVersion);
        }

        public async Task Delete(ObjectId agentVersionId)
        {
            Assert.MustNotNull(_identityProvider.User, "Not logged!");

            await AgentVersion
                .DeleteAsync(n => n.AgentVersionId == agentVersionId);
        }
    }
}
