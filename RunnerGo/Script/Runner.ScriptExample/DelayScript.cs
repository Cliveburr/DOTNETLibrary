using Runner.Script.Interface.Scripts;

namespace Runner.ScriptExample
{
    [Script(0, "Delay")]
    public class DelayScript : IScript
    {
        public async Task Run(ScriptRunContext context)
        {
            var delaySeconds = context.Data.GetString("DelaySeconds");

            if (int.TryParse(delaySeconds, out var delay))
            {
                await Task.Delay(delay);
            }
        }
    }
}
