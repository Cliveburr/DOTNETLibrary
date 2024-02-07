using Runner.Script.Interface.Scripts;

namespace Runner.ScriptExample
{
    [Script(1, "HelloWorld")]
    public class SayHelloWorld : IScript
    {
        public async Task Run(ScriptRunContext context)
        {
            await context.Log("say hello...");

            context.Data.SetString("SomeValue", "changed");
        }
    }
}
