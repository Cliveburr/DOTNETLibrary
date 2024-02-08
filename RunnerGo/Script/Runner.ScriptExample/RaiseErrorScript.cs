using Runner.Script.Interface.Scripts;

namespace Runner.ScriptExample
{
    [Script(2, "RaiseError")]
    public class RaiseErrorScript : IScript
    {
        public Task Run(ScriptRunContext context)
        {
            var errorMessage = context.Data.GetString("ErrorMessage");

            if (!string.IsNullOrEmpty(errorMessage))
            {
                throw new Exception(errorMessage);
            }

            return Task.CompletedTask;
        }
    }
}
