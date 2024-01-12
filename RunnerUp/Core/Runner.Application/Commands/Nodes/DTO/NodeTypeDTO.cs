
namespace Runner.Application.Commands.Nodes.DTO
{
    public enum NodeTypeDTO : byte
    {
        App = 0,
        Folder = 1,
        AgentPool = 2,
        Agent = 3,
        Flow = 4,
        Run = 5,
        Data = 6,
        DataType = 7,
        ScriptPackage = 8,
        Script = 9
    }

    public enum CreableNodeType : byte
    {
        Folder = 1,
        AgentPool = 2,
        Flow = 4,
        Data = 6,
        DataType = 7,
        ScriptPackage = 8
    }
}
