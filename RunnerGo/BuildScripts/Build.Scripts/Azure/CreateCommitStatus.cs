using Runner.Script.Interface.Scripts;
using System.ComponentModel.DataAnnotations;

namespace Build.Scripts.Azure
{
    public class CreateCommitStatusInputData
    {
        [Required]
        public required string Uri { get; set; }
        [Required]
        public required string Token { get; set; }
        [Required]
        public required string Project { get; set; }
        [Required]
        public required string Repository { get; set; }
        [Required]
        public required string FlowPath { get; set; }
    }

    [Script(0, "BuildSuccessStatus", typeof(CreateCommitStatusInputData))]
    public class BuildSuccessStatus : IScript
    {
        public async Task Run(ScriptRunContext context)
        {
            var input = context.Data.ReadInput<CreateCommitStatusInputData>();
            CheckAssert.MustNotNull(input?.Uri, "Invalid Uri!");
            CheckAssert.MustNotNull(input.Token, "Invalid Token!");
            CheckAssert.MustNotNull(input.Project, "Invalid Project!");
            CheckAssert.MustNotNull(input.Repository, "Invalid Repository!");
            CheckAssert.MustNotNull(input.FlowPath, "Invalid BranchName!");
            CheckAssert.Complete();

            var sourcePath = context.Workspace.GetFolder("s");
            var description = "Build succeeded";
            var contextName = "Runner";
            var contextGenre = input.FlowPath;
            var targetUrl = string.Join('/', context.SiteUrl.Trim('/'), "app", input.FlowPath);

            using (var azureAccess = new AzureAccess(input.Uri, input.Token))
            {
                await azureAccess.Connect();

                await azureAccess.CreateCommitStatus(input.Project, input.Repository, sourcePath.Path, true,
                    description, contextName, contextGenre, targetUrl);
            }
        }
    }

    [Script(0, "BuildFailedStatus", typeof(CreateCommitStatusInputData))]
    public class BuildFailedStatus : IScript
    {
        public async Task Run(ScriptRunContext context)
        {
            var input = context.Data.ReadInput<CreateCommitStatusInputData>();
            CheckAssert.MustNotNull(input?.Uri, "Invalid Uri!");
            CheckAssert.MustNotNull(input.Token, "Invalid Token!");
            CheckAssert.MustNotNull(input.Project, "Invalid Project!");
            CheckAssert.MustNotNull(input.Repository, "Invalid Repository!");
            CheckAssert.MustNotNull(input.FlowPath, "Invalid BranchName!");
            CheckAssert.Complete();

            var sourcePath = context.Workspace.GetFolder("s");
            var description = "Build succeeded";
            var contextName = "Runner";
            var contextGenre = input.FlowPath;
            var targetUrl = string.Join('/', context.SiteUrl.Trim('/'), "app", input.FlowPath.Trim('/'));

            using (var azureAccess = new AzureAccess(input.Uri, input.Token))
            {
                await azureAccess.Connect();

                await azureAccess.CreateCommitStatus(input.Project, input.Repository, sourcePath.Path, false,
                    description, contextName, contextGenre, targetUrl);
            }
        }
    }
}
