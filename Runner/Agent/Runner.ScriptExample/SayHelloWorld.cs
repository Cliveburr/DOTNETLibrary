using Runner.Agent.Interface.Scripts;

namespace Runner.ScriptExample
{
    public class SayHelloWorld : IScript
    {
        public async Task Run(ScriptRunContext context)
        {
            await context.Log("say hello...");

            context.Output.SetString("SomeValue", "123");
        }
    }
}
