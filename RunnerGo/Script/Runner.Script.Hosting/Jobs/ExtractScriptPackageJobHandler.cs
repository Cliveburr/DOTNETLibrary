using Runner.Agent.Version.Scripts;
using Runner.Business.Entities.Job;
using Runner.Business.Jobs;
using Runner.Business.Services.NodeTypes;
using Runner.Common.Helpers;
using System.Text;

namespace Runner.Script.Hosting.Jobs
{
    public class ExtractScriptPackageJobHandler : IJobHandler
    {
        private readonly ScriptContentService _scriptContentService;
        private readonly ScriptPackageService _scriptPackageService;

        public ExtractScriptPackageJobHandler(ScriptContentService scriptContentService, ScriptPackageService scriptPackageService)
        {
            _scriptContentService = scriptContentService;
            _scriptPackageService = scriptPackageService;
        }

        public async Task<bool> Execute(Job job)
        {
            Assert.MustNotNull(job.ScriptContentId, "ExtractScriptPackageJobHandler missing ScriptContentId in job of type ExtractScriptPackage!");
            Assert.MustNotNull(job.ScriptPackageId, "ExtractScriptPackageJobHandler missing ScriptPackageId in job of type ExtractScriptPackage!");

            var warnings = new StringBuilder();
            var cleanScriptContent = true;

            try
            {
                var scriptContent = await _scriptContentService.ReadById(job.ScriptContentId.Value);
                Assert.MustNotNull(scriptContent, $"ScriptContent in job of type ExtractScriptPackage! ScriptContentId: {job.ScriptContentId}");

                using (var temp = TempDirectory.Create("ExtractScriptPackage"))
                {
                    Zip.Descompat(scriptContent.FileContent, temp.Path);

                    var isolation = new ScriptIsolation(temp.Path);
                    var scriptSets = isolation.Execute(warnings);

                    cleanScriptContent = false;
                    await _scriptPackageService.SetScripts(job.ScriptPackageId.Value, scriptContent.ScriptContentId, scriptSets, warnings);
                }

                return true;
            }
            catch (Exception ex)
            {
                warnings.AppendLine("Error: " + ex.Message);

                if (cleanScriptContent)
                {
                    try
                    {
                        await _scriptContentService.Delete(job.ScriptContentId.Value);
                    }
                    catch (Exception ex2)
                    {
                        warnings.AppendLine("Error: " + ex2.Message);
                    }
                }
                throw;
            }
            finally
            {
                if (warnings.Length > 0)
                {
                    try
                    {
                        await _scriptPackageService.UpdateWarningAndClearJob(job.ScriptPackageId.Value, warnings);
                    }
                    catch
                    { }
                }
            }
        }
    }
}
