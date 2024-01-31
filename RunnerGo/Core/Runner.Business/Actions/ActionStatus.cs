
namespace Runner.Business.Actions
{
    /*

        Run - marca uma ação que esteja com cursor para ser executada
            from: Waiting, Stopped, Error
            to: ToRun

        SetRunning - marca uma ação sendo executada
            from: ToRun
            to: Running

        SetCompleted - marca uma ação que estava sendo executada como completada, move o cursor
            from: Running
            to: Completed

        SetError - marca uma ação que estava sendo executada como error
            from: Running
            to: Error

        Stop - marca uma ação rodando para parar
            from: Running
            to: ToStop

        SetStopped - marca uma ação marcada para parar, como parado
            from: ToStop
            to: Stopped

    */

    public enum ActionStatus : byte
    {
        Waiting = 0,
        ToRun = 1,
        Running = 2,
        ToStop = 3,
        Stopped = 4,
        Error = 5,
        Completed = 6
    }
}
