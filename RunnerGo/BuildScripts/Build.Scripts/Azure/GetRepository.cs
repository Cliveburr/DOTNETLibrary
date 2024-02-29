using Newtonsoft.Json.Linq;
using Runner.Script.Interface.Scripts;
using System;
using System.ComponentModel.DataAnnotations;
using System.IO;

namespace Build.Scripts.Azure
{
    public class GetRepositoryInputData
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
        public required string BranchName { get; set; }
    }

    [Script(0, "GetRepository", typeof(GetRepositoryInputData))]
    public class GetRepository : IScript
    {
        public async Task Run(ScriptRunContext context)
        {
            var input = context.Data.ReadInput<GetRepositoryInputData>();
            CheckAssert.MustNotNull(input?.Uri, "Invalid Uri!");
            CheckAssert.MustNotNull(input.Token, "Invalid Token!");
            CheckAssert.MustNotNull(input.Project, "Invalid Project!");
            CheckAssert.MustNotNull(input.Repository, "Invalid Repository!");
            CheckAssert.MustNotNull(input.BranchName, "Invalid BranchName!");
            CheckAssert.Complete();

            var sourcePath = context.Workspace.GetFolder("s");

            using (var azureAccess = new AzureAccess(input.Uri, input.Token))
            {
                await azureAccess.Connect();

                await azureAccess.GetRepository(input.Project, input.Repository, input.BranchName, sourcePath.Path);
            }
        }
    }
}
