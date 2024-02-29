using Runner.Script.Interface.Scripts;

namespace Build.Scripts.Azure
{
    public class CreateCommitStatusInputData
    {

    }

    [Script(0, "CreateCommitStatus", typeof(CreateCommitStatusInputData))]
    public class CreateCommitStatus : IScript
    {
        public async Task Run(ScriptRunContext context)
        {
            var input = context.Data.ReadInput<CreateCommitStatusInputData>();
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

                await azureAccess.CreateCommitStatus(input.Project, input.Repository, input.BranchName, sourcePath.Path);
            }
        }
    }
}
