using Runner.Script.Interface.Scripts;

namespace Runner.ScriptExample
{
    [Script(0, "RaiseError")]
    public class RaiseErrorScript : IScript
    {
        public async Task Run(ScriptRunContext context)
        {
            var errorMessage = context.Data.GetString("ErrorMessage");

            if (!string.IsNullOrEmpty(errorMessage))
            {
                throw new Exception(errorMessage);
            }
        }
    }
}
