using Microsoft.Extensions.DependencyInjection;
using MongoDB.Bson;
using Runner.Agent.Hosting.Model;
using Runner.Agent.Hosting.Services;
using Runner.Agent.Interface.Model;
using Runner.Agent.Interface.Model.Data;
using Runner.Business.Actions;
using Runner.Business.Datas.Model;
using Runner.Business.Entities.Job;
using Runner.Business.Jobs;
using Runner.Business.Services;
using Runner.Business.Services.NodeTypes;

namespace Runner.Agent.Hosting.Jobs
{
    public class RunScriptJobHandler : IJobHandler
    {
        private readonly RunService _runService;
        private readonly DataExpandService _dataExpandService;
        private readonly ScriptService _scriptService;
        private readonly AgentService _agentService;
        private readonly AgentManagerService _agentManagerService;

        public RunScriptJobHandler(RunService runService, DataExpandService dataExpandService, ScriptService scriptService, AgentService agentService, AgentManagerService agentManagerService)
        {
            _runService = runService;
            _dataExpandService = dataExpandService;
            _scriptService = scriptService;
            _agentService = agentService;
            _agentManagerService = agentManagerService;
        }

        public async Task<bool> Execute(Job job)
        {
            Assert.MustNotNull(job.RunId, "RunScriptJobHandler missing RunId, JobId: " + job.JobId);
            Assert.MustNotNull(job.ActionId, "RunScriptJobHandler missing ActionId, JobId: " + job.JobId);

            try
            {
                var run = await _runService.ReadById(job.RunId.Value);
                Assert.MustNotNull(run, "RunActionJob invalid RunId, JobId: " + job.JobId);

                var control = ActionControl.From(run);
                var contextData = control.ComputeActionContextData(job.ActionId.Value, _dataExpandService);

                var scriptPath = contextData.ReadScriptVersion("Script");
                Assert.MustNotNull(scriptPath, $"Run with invalid script path! {{ RunId: {job.RunId.Value}, ActionId: {job.ActionId.Value} }}");
                var agentPoolNodeId = contextData.ReadNodePath("AgentPool");
                Assert.MustNotNull(agentPoolNodeId, $"Run with invalid agent pool path! {{ RunId: {job.RunId.Value}, ActionId: {job.ActionId.Value} }}");

                var sts = await _scriptService.ReadVersionByScriptPath(scriptPath.Value.ScriptNodeId, scriptPath.Value.Version);
                Assert.MustNotNull(sts, $"Run with invalid script path! {{ RunId: {job.RunId.Value}, ActionId: {job.ActionId.Value}, ScriptPath: {scriptPath} }}");

                if (sts.Value.ScriptVersion.Input?.Any() ?? false)
                {
                    contextData.Merge(sts.Value.ScriptVersion.Input);
                    await contextData.Resolve();
                    var validation = contextData.Validate();
                    if (validation.Any())
                    {
                        var fullMsg = string.Join(Environment.NewLine, validation.Select(v => v.Text));
                        throw new RunnerException(fullMsg);
                    }
                }
                else
                {
                    await contextData.Resolve();
                }

                var actionTags = contextData.ReadStringList("Tags")
                    ?? new List<string>();

                var agentConnected = await FindFreeAgentConnected(job, agentPoolNodeId.Value, actionTags);
                if (agentConnected is not null)
                {
                    try
                    {
                        var runAgentService = agentConnected.Scope.ServiceProvider.GetRequiredService<RunService>();
                        await runAgentService.SetRunning(job.RunId.Value, job.ActionId.Value);

                        var jobAgentService = agentConnected.Scope.ServiceProvider.GetRequiredService<JobService>();
                        await jobAgentService.SetRunning(job.JobId, agentConnected.AgentId);

                        await _agentManagerService.CallRunScript(agentConnected.ConnectionId, new RunScriptRequest
                        {
                            ScriptContentId = sts.Value.ScriptVersion.ScriptContentId.ToString(),
                            Assembly = sts.Value.ScriptVersion.Assembly,
                            FullTypeName = sts.Value.ScriptVersion.FullTypeName,
                            InputData = MapDataPropertyToAgent(contextData.ToDataProperty())
                        });

                        return true;
                    }
                    catch
                    {
                        agentConnected.JobRunning = null;
                        _agentManagerService.FlagToCheckJobsWaiting();
                        throw;
                    }
                }
            }
            catch (Exception ex)
            {
                try
                {
                    await _runService.SetError(job.RunId.Value, job.ActionId.Value, ex.Message, ex.ToString());
                }
                catch (Exception ex2)
                {
                    throw new AggregateException(ex, ex2);
                }
                throw;
            }

            return false;
        }

        private async Task<AgentConnect?> FindFreeAgentConnected(Job job, ObjectId agentPoolNodeId, List<string> actionTags)
        {
            var agents = await _agentService.ReadAgentsByAgentPoolNodeId(agentPoolNodeId);
            Assert.MustNotNull(agents, $"Run with invalid agent pool path! {{ RunId: {job.RunId!.Value}, ActionId: {job.ActionId!.Value}, AgentPoolPath: {agentPoolNodeId} }}");

            foreach (var agent in agents)
            {
                var agentTags = agent.RegistredTags.ToList();
                if (agent.ExtraTags is not null)
                {
                    agentTags.AddRange(agent.ExtraTags);
                }

                var agentConnected = _agentManagerService.FindAgent(a =>
                {
                    if (a.JobRunning is not null)
                    {
                        return false;
                    }

                    if (a.AgentId != agent.AgentId)
                    {
                        return false;
                    }

                    if (agentTags.Intersect(actionTags).Count() != agentTags.Count)
                    {
                        return false;
                    }

                    a.JobRunning = new JobRunning
                    {
                        JobId = job.JobId,
                        RunId = job.RunId.Value,
                        ActionId = job.ActionId.Value
                    };

                    return true;
                });

                if (agentConnected is not null)
                {
                    return agentConnected;
                }
            }

            return null;
        }

        private List<AgentDataProperty>? MapDataPropertyToAgent(List<DataProperty>? properties)
        {
            return properties?
                .Select(p => new AgentDataProperty
                {
                    Name = p.Name,
                    Type = (AgentDataTypeEnum)p.Type,
                    Value = p.Value is null ?
                        null :
                        new AgentDataValue
                        {
                            StringValue = p.Value.StringValue,
                            IntValue = p.Value.IntValue,
                            StringListValue = p.Value.StringListValue,
                            NodePath = p.Value.NodePath,
                            DataExpand = MapDataPropertyToAgent(p.Value.DataExpand?.Select(d => d.ToDataProperty()).ToList())
                        }
                })
                .ToList();
        }
    }
}
