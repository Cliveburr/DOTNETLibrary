using Build.Scripts.Helpers;
using Runner.Script.Interface.Scripts;
using System.ComponentModel.DataAnnotations;

namespace Build.Scripts.MSBuild
{
    public class MSBuildCommandInputData
    {
        [Required]
        public required string Project { get; set; }
        public string? Platform { get; set; }
        public string? Configuration { get; set; }
        public string? VisualStudioVersion { get; set; }
        public string? Arguments { get; set; }
    }

    [Script(0, "MSBuildCommand", typeof(MSBuildCommandInputData))]
    public class MSBuildCommand : IScript
    {
        public Task Run(ScriptRunContext context)
        {
            var input = context.Data.ReadInput<MSBuildCommandInputData>();
            CheckAssert.MustNotNull(input?.Project, "Invalid Project!");
            CheckAssert.Complete();

            var sourcePath = context.Workspace.GetFolder("s");
            var fullProject = Path.Combine(sourcePath.Path, input.Project);
            if (!File.Exists(fullProject))
            {
                throw new FileNotFoundException(fullProject);
            }

            var msbuildPath = MSBuildLocation.GetMSBuildPath();

            var projectWorkingFolder = Path.GetDirectoryName(fullProject)!;

            var arguments = new List<string>
            {
                $"\"{fullProject}\"",
                "/nologo",
                "/nr:false"
            };

            if (!string.IsNullOrEmpty(input.Platform))
            {
                arguments.Add($"/p:platform=\"{input.Platform}\"");
            }
            if (!string.IsNullOrEmpty(input.Configuration))
            {
                arguments.Add($"/p:configuration=\"{input.Configuration}\"");
            }
            if (!string.IsNullOrEmpty(input.VisualStudioVersion))
            {
                arguments.Add($"/p:VisualStudioVersion=\"{input.VisualStudioVersion}\"");
            }
            if (!string.IsNullOrEmpty(input.Arguments))
            {
                arguments.Add(input.Arguments);
            }

            var process = new ProccessHelper(msbuildPath, projectWorkingFolder, string.Join(' ', arguments), context.Log);
            process.Run();

            return Task.CompletedTask;
        }
    }
}
