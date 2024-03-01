using Build.Scripts.Helpers;
using Runner.Script.Interface.Scripts;
using System.ComponentModel.DataAnnotations;

namespace Build.Scripts.Nuget
{
    public class NugetRestoreInputData
    {
        [Required]
        public required string SolutionFile { get; set; }
        public string? ConfigFile { get; set; }
    }

    [Script(0, "NugetRestore", typeof(NugetRestoreInputData))]
    public class NugetRestore : IScript
    {
        public Task Run(ScriptRunContext context)
        {
            var input = context.Data.ReadInput<NugetRestoreInputData>();
            CheckAssert.MustNotNull(input?.SolutionFile, "Invalid SolutionFile!");
            CheckAssert.Complete();

            var sourcePath = context.Workspace.GetFolder("s");
            var fullSolutionPath = Path.Combine(sourcePath.Path, input.SolutionFile);
            if (!File.Exists(fullSolutionPath))
            {
                throw new FileNotFoundException(fullSolutionPath);
            }

            var argument = $"restore \"{fullSolutionPath}\" -Verbosity Detailed -NonInteractive";

            if (!string.IsNullOrEmpty(input.ConfigFile))
            {
                var fullConfigFilePath = Path.Combine(fullSolutionPath, input.ConfigFile);
                if (!File.Exists(fullConfigFilePath))
                {
                    throw new FileNotFoundException(fullConfigFilePath);
                }

                argument += $" -ConfigFile \"{fullConfigFilePath}\"";
            }

            var nugetPath = NugetLocation.GetNugetPath();
            if (!File.Exists(nugetPath))
            {
                throw new FileNotFoundException(nugetPath);
            }

            var process = new ProccessHelper(nugetPath, argument, context.Log);
            process.Run();

            return Task.CompletedTask;
        }
    }
}
