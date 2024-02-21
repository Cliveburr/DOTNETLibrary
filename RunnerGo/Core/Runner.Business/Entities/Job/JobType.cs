
namespace Runner.Business.Entities.Job
{
    public enum JobType : byte
    {
        AgentUpdate = 0,
        ExtractScriptPackage = 1,
        RunScript = 2,
        StopScript = 3,
        CreateRun = 4,
        CleanRunRetain = 5,  //TODO
        CleanJobs = 6        //TODO
    }
}
