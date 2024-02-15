
namespace Runner.Business.Datas2.Model
{
    public enum DataTypeEnum : byte
    {
        String = 0,                // property with StringValue
        StringList = 1,            // property with StringListValue
        Node = 2,                  // property with ObjectIdValue to refer some node, like AgentPool

        Inherit = 3,               // property with ObjectIdValue to refer data node
                                   // will be merged in resolve
        Data = 4,                  // property with ObjectIdValue to refer data node
                                   // will be loaded into DataExpand in resolve
        ScriptVersion = 5          // property with ObjectIdValue and IntValue to refer ScriptVersion
                                   // will be merged after changed
    }
}