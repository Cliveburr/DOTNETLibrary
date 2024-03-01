using Build.Scripts.Helpers;
using Runner.Script.Interface.Scripts;
using System.ComponentModel.DataAnnotations;

namespace Build.Scripts.Nodejs
{
    public class NpmCommandInputData
    {
        [Required]
        public required string WorkingFolder { get; set; }
        [Required]
        public required string Command { get; set; }
    }

    [Script(0, "NpmCommand", typeof(NpmCommandInputData))]
    public class NpmCommand : IScript
    {
        public Task Run(ScriptRunContext context)
        {
            var input = context.Data.ReadInput<NpmCommandInputData>();
            CheckAssert.MustNotNull(input?.WorkingFolder, "Invalid WorkingFolder!");
            CheckAssert.MustNotNull(input.Command, "Invalid Command!");
            CheckAssert.Complete();

            var sourcePath = context.Workspace.GetFolder("s");
            var fullWorkingFolder = Path.Combine(sourcePath.Path, input.WorkingFolder);
            if (!Directory.Exists(fullWorkingFolder))
            {
                throw new DirectoryNotFoundException(fullWorkingFolder);
            }

            var npmPath = NodejsLocation.GetNpmPath();
            if (!File.Exists(npmPath))
            {
                throw new FileNotFoundException(npmPath);
            }

            var process = new ProccessHelper(npmPath, fullWorkingFolder, input.Command, context.Log);
            process.Run();

            return Task.CompletedTask;
        }
    }
}
