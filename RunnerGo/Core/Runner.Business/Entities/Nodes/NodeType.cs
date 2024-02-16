
namespace Runner.Business.Entities.Nodes
{
    public enum NodeType : byte
    {
        App = 0,
        Folder = 1,
        Data = 2,
        //DataType = 3,
        AgentPool = 4,
        Agent = 5,
        ScriptPackage = 6,
        Script = 7,
        Flow = 8,
        Run = 9
    }
}
