using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Nodus.Update.WindowsApi
{
    public static class Functions
    {
        [DllImport("advapi32.dll", EntryPoint = "OpenSCManagerW", ExactSpelling = true, CharSet = CharSet.Unicode, SetLastError = true)]
        public static extern IntPtr OpenSCManager(string machineName, string databaseName, AccessRights dwDesiredAccess);

        [DllImport("advapi32.dll", EntryPoint = "EnumServicesStatusW", ExactSpelling = true, SetLastError = true)]
        public static extern bool EnumServicesStatus(IntPtr hSCManager, ServicesTypes dwServiceType, ServicesState dwServiceState, IntPtr lpServices, int cbBufSize, ref int pcbBytesNeeded, ref int lpServicesReturned, ref int lpResumeHandle);

        [DllImport("advapi32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool CloseServiceHandle(IntPtr hSCObject);

        [DllImport("advapi32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern IntPtr OpenService(IntPtr hSCManager, string lpServiceName, AccessRights dwDesiredAccess);

        [DllImport("kernel32.dll")]
        public static extern uint GetLastError();
        [DllImport("advapi32.dll")]
        public static extern int ControlService(IntPtr hService, ServiceControl dwControl, ref ServiceStatus lpServiceStatus);

        [DllImport("advapi32.dll", SetLastError = true)]
        public static extern int StartService(IntPtr hService, int dwNumServiceArgs, int lpServiceArgVectors);

        [DllImport("advapi32.dll")]
        public static extern int QueryServiceStatus(IntPtr hService, ref ServiceStatus lpServiceStatus);
    }
}