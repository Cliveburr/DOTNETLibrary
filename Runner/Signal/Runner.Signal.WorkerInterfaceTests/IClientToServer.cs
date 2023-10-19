namespace Runner.Signal.WorkerInterfaceTests
{
    public interface IClientToServer
    {
        Task<string> Ping();
    }
}