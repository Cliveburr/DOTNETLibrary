using Build.Scripts.Helpers;
using Runner.Script.Interface.Scripts;
using System.ComponentModel.DataAnnotations;

namespace Build.Scripts
{
    public class CommandLineInputData
    {
        [Required]
        public required string FileName { get; set; }
        public string? Argument { get; set; }
    }

    [Script(0, "CommandLine", typeof(CommandLineInputData))]
    public class CommandLine : IScript
    {
        public Task Run(ScriptRunContext context)
        {
            var input = context.Data.ReadInput<CommandLineInputData>();
            CheckAssert.MustNotNull(input?.FileName, "Invalid FileName!");
            CheckAssert.Complete();

            var sourcePath = context.Workspace.GetFolder("s");
            var fullFileName = Path.Combine(sourcePath.Path, input.FileName);
            if (!Directory.Exists(fullFileName))
            {
                throw new DirectoryNotFoundException(fullFileName);
            }

            var process = new ProccessHelper(input.FileName, input.Argument ?? "", context.Log);
            process.Run();

            return Task.CompletedTask;
        }
    }
}
