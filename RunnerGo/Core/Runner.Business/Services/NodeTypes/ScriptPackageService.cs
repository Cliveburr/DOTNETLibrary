using MongoDB.Bson;
using Runner.Business.DataAccess;
using Runner.Business.Entities.Nodes.Types;
using Runner.Business.Entities.Nodes;
using Runner.Business.Security;
using MongoDB.Driver;
using Runner.Business.Entities.Job;
using Runner.Business.Model.Nodes.Types;
using System.Xml.Linq;

namespace Runner.Business.Services.NodeTypes
{
    public class ScriptPackageService : DataServiceBase
    {
        private readonly IdentityProvider _identityProvider;
        private readonly NodeService _nodeService;
        private readonly JobService _jobService;

        public ScriptPackageService(Database database, IdentityProvider identityProvider, NodeService nodeService, JobService jobService)
            : base(database)
        {
            _identityProvider = identityProvider;
            _nodeService = nodeService;
            _jobService = jobService;
        }

        public Task<ScriptPackage?> ReadById(ObjectId scriptPackageId)
        {
            return ScriptPackage
                .FirstOrDefaultAsync(sp => sp.ScriptPackageId == scriptPackageId);
        }

        public Task<ScriptPackage?> ReadByNodeId(ObjectId nodeId)
        {
            return ScriptPackage
                .FirstOrDefaultAsync(sp => sp.NodeId == nodeId);
        }

        public async Task Create(string? name, ObjectId parentId)
        {
            Assert.MustNotNull(_identityProvider.User, "Not logged!");

            var has = await _nodeService.ReadByNameAndParent(name, parentId);
            Assert.MustNull(has, "ScriptPackage name already exist!");

            _nodeService.ValidateName(name);

            var node = new Node
            {
                Type = NodeType.ScriptPackage,
                Name = name,
                ParentId = parentId,
                CreatedUtc = DateTime.UtcNow,
                UpdatedUtc = DateTime.UtcNow
            };
            await Node
                .InsertAsync(node);

            await _nodeService.UpdateUtc(parentId);

            var scriptPackage = new ScriptPackage
            {
                NodeId = node.NodeId
            };
            await ScriptPackage
                .InsertAsync(scriptPackage);
        }

        public async Task DeleteByNode(Node node)
        {
            Assert.MustNotNull(_identityProvider.User, "Not logged!");

            await ScriptPackage
                .DeleteAsync(a => a.NodeId == node.NodeId);

            if (node.ParentId.HasValue)
            {
                await _nodeService.UpdateUtc(node.ParentId.Value);
            }

            await Node
                .DeleteAsync(n => n.NodeId == node.NodeId);
        }

        public async Task ExtractScripts(ObjectId scriptPackageNodeId, string fileName, byte[] fileContent)
        {
            Assert.MustNotNull(_identityProvider.User, "Need to be logged to upload a script package!");

            var scriptPackage = await ReadByNodeId(scriptPackageNodeId);
            Assert.MustNotNull(scriptPackage, $"Invalid ScriptPackageNodeId! {scriptPackageNodeId}");

            Assert.MustNull(scriptPackage.ExtractJobId, "ScriptPackage already in extract scripts process...");

            var scriptContent = new ScriptContent
            {
                CreatedUtc = DateTime.UtcNow,
                FileName = fileName,
                FileContent = fileContent
            };
            await ScriptContent
                .InsertAsync(scriptContent);

            var job = await _jobService.AddExtractScriptPackage(scriptContent.ScriptContentId, scriptPackage.ScriptPackageId);

            var scriptPackageUpdate = Builders<ScriptPackage>.Update
                .Set(sp => sp.ExtractJobId, job.JobId);
            await ScriptPackage
                .UpdateAsync(sp => sp.ScriptPackageId == scriptPackage.ScriptPackageId, scriptPackageUpdate);
        }

        public async Task SetScripts(ObjectId scriptPackageId, ObjectId scriptContentId, List<ScriptSet> scriptSets)
        {
            var scriptPackage = await ReadById(scriptPackageId);
            Assert.MustNotNull(scriptPackage, $"Invalid ScriptPackageId! {scriptPackageId}");

            var scriptPackageUpdate = Builders<ScriptPackage>.Update
                .Set(sp => sp.ExtractJobId, null);
            await ScriptPackage
                .UpdateAsync(sp => sp.ScriptPackageId == scriptPackage.ScriptPackageId, scriptPackageUpdate);

            if (scriptSets.Count == 0)
            {
                await ScriptContent
                    .DeleteAsync(sc => sc.ScriptContentId == scriptContentId);
                return;
            }

            foreach (var scriptSet in scriptSets)
            {
                try
                {
                    _nodeService.ValidateName(scriptSet.Name);
                }
                catch
                {
                    continue;
                }

                var node = await _nodeService.ReadChildByName(scriptPackage.NodeId, scriptSet.Name);
                if (node is null)
                {
                    if (scriptSet.Version == 0)
                    {
                        node = new Node
                        {
                            Type = NodeType.Script,
                            Name = scriptSet.Name,
                            ParentId = scriptPackage.NodeId,
                            CreatedUtc = DateTime.UtcNow,
                            UpdatedUtc = DateTime.UtcNow
                        };
                        await Node
                            .InsertAsync(node);

                        var script = new Script
                        {
                            NodeId = node.NodeId,
                            NextVersion = 1,
                            Versions = new List<ScriptVersion>
                        {
                            new ScriptVersion
                            {
                                Version = scriptSet.Version,
                                ScriptContentId = scriptContentId,
                                Assembly = scriptSet.Assembly,
                                FullTypeName = scriptSet.FullTypeName,
                                InputTypes = scriptSet.InputTypes,
                                OutputTypes = scriptSet.OutputTypes
                            }
                        }
                        };
                        await Script
                            .InsertAsync(script);
                    }
                    else
                    {
                        //TODO: log, first package need set version = 0
                    }
                }
                else
                {
                    var script = await Script
                        .FirstOrDefaultAsync(s => s.NodeId == node.NodeId);
                    Assert.MustNotNull(script, $"Invalid ScriptNodeId! {node.NodeId}");

                    if (scriptSet.Version == script.NextVersion)
                    {
                        await _nodeService.UpdateUtc(node.NodeId);

                        script.NextVersion++;
                        script.Versions.Add(new ScriptVersion
                        {
                            Version = scriptSet.Version,
                            ScriptContentId = scriptContentId,
                            Assembly = scriptSet.Assembly,
                            FullTypeName = scriptSet.FullTypeName,
                            InputTypes = scriptSet.InputTypes,
                            OutputTypes = scriptSet.OutputTypes
                        });

                        await Script
                            .ReplaceAsync(s => s.ScriptId == script.ScriptId, script);
                    }
                    else
                    {
                        //TODO: log, the script version is updated only if the version == nextVersion
                    }
                }
            }

            await _nodeService.UpdateUtc(scriptPackage.NodeId);
        }
    }
}
