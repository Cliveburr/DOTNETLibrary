using Runner.Script.Interface.Scripts;
using System.ComponentModel.DataAnnotations;

namespace Runner.ScriptExample
{
    public class DelayInputData
    {
        [Required]
        public string? DelaySeconds { get; set; }
    }

    [Script(0, "Delay", typeof(DelayInputData))]
    public class DelayScript : IScript
    {
        public async Task Run(ScriptRunContext context)
        {
            var input = context.Data.ReadInput<DelayInputData>();
            if (input?.DelaySeconds is null)
            {
                throw new Exception("Missing input!");
            }

            if (int.TryParse(input.DelaySeconds, out var delay))
            {
                await Task.Delay(delay * 1000);
            }
        }
    }
}
