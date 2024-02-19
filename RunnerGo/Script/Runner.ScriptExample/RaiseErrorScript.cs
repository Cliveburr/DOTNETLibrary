using Runner.Script.Interface.Scripts;
using System.ComponentModel.DataAnnotations;

namespace Runner.ScriptExample
{
    public class RaiseErrorInputData
    {
        [Required]
        public string? ErrorMessage { get; set; }
    }

    [Script(0, "RaiseError", typeof(RaiseErrorInputData))]
    public class RaiseErrorScript : IScript
    {
        public Task Run(ScriptRunContext context)
        {
            var input = context.Data.ReadInput<RaiseErrorInputData>();

            if (input is not null && !string.IsNullOrEmpty(input.ErrorMessage))
            {
                throw new Exception(input.ErrorMessage);
            }

            return Task.CompletedTask;
        }
    }
}
