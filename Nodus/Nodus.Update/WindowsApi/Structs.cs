using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Nodus.Update.WindowsApi
{
    [Flags]
    public enum AccessRights
    {
        StandardRightsRequired = 0xF003F,
        Connect = 0x0001,
        CreateService = 0x0002,
        EnumerateService = 0x0004,
        Lock = 0x0008,
        QueryLockStatus = 0x0010,
        ModifyBootConfig = 0x0020,
        ServicePauseContinue = 0x0040,
        ServiceInterrogate = 0x0080,
        ServiceUserDefinedControl = 0X0100,
        AllAccess = (StandardRightsRequired | Connect | CreateService |
                     EnumerateService | Lock | QueryLockStatus | ModifyBootConfig),
        Delete = (StandardRightsRequired | Connect | CreateService |
                     EnumerateService | Lock | QueryLockStatus | ModifyBootConfig |
                     ServicePauseContinue | ServiceInterrogate | ServiceUserDefinedControl),
        Read = (StandardRightsRequired | EnumerateService | QueryLockStatus)
    }

    [Flags]
    public enum ServicesTypes : int
    {
        KernelDriver = 0x00000001,
        FileSystemDriver = 0x00000002,
        Win32OwnProcess = 0x00000010,
        Win32ShareProcess = 0x00000020,
        InteractiveProcess = 0x00000100,
        Win32 = Win32OwnProcess | Win32ShareProcess
    }

    public enum ServicesState : int
    {
        Unknown = -1, // The state cannot be (has not been) retrieved.
        NotFound = 0, // The service is not known on the host server.
        Stopped = 1,
        StartPending = 2,
        StopPending = 3,
        Running = 4,
        ContinuePending = 5,
        PausePending = 6,
        Paused = 7,
    }

    public enum ServiceControl : uint
    {
        Stop = 0x00000001,
        Pause = 0x00000002,
        Continue = 0x00000003,
        Interrogate = 0x00000004,
        Shutdown = 0x00000005,
        ParamChange = 0x00000006,
        NetBindAdd = 0x00000007,
        NetBindRemove = 0x00000008,
        NetBindEnable = 0x00000009,
        NetBindDisable = 0x0000000A
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    public struct EnumServiceStatus
    {
        [MarshalAs(System.Runtime.InteropServices.UnmanagedType.LPWStr)]
        public string pServiceName;
        [MarshalAs(System.Runtime.InteropServices.UnmanagedType.LPWStr)]
        public string pDisplayName;
        public ServiceStatus ServiceStatus;
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    public struct ServiceStatus
    {
        public ServicesTypes dwServiceType;
        public ServicesState dwCurrentState;
        public uint dwControlsAccepted;
        public uint dwWin32ExitCode;
        public uint dwServiceSpecificExitCode;
        public uint dwCheckPoint;
        public uint dwWaitHint;
    }
}