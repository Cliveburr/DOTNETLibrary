
namespace Runner.Script.Interface.Scripts
{
    public interface IWorkspace
    {
        string BasePath { get; }
        IWorkspaceFolder GetFolder(string name);
        IWorkspaceTempFolder GetTemp();
    }

    public interface IWorkspaceFolder
    {
        string Path { get; }
        void ClearDirectory();
        void ClearDirectory(string subPath);
        void DeleteDirectory();
        void DeleteDirectory(string subPath);
    }

    public interface IWorkspaceTempFolder : IWorkspaceFolder
    {
    }
}
