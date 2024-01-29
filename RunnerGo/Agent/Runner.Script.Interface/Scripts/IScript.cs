
namespace Runner.Script.Interface.Scripts
{
    public interface IScript
    {
        Task Run(ScriptRunContext context);
    }
}
