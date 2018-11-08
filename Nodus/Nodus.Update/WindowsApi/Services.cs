using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Nodus.Update.WindowsApi
{
    public class Services : IDisposable
    {
        private IntPtr _scManager = (IntPtr)0;

        public Services()
        {
            OpenSCManage();
        }

        private void OpenSCManage()
        {
            _scManager = Functions.OpenSCManager(null, null, AccessRights.AllAccess);
            if (_scManager == IntPtr.Zero)
                throw new Exception("Can't OpenSCManager!");
        }

        public void Dispose()
        {
            Functions.CloseServiceHandle(_scManager);
        }

        public EnumServiceStatus[] GetAllServices()
        {
            if (_scManager == IntPtr.Zero)
                return null;

            var buffer = (IntPtr)0;
            try
            {
                int iBytesNeeded = 0, iServicesReturned = 0, iResumeHandle = 0;
                Functions.EnumServicesStatus(_scManager, ServicesTypes.Win32, ServicesState.Stopped | ServicesState.StartPending, IntPtr.Zero, 0, ref iBytesNeeded, ref iServicesReturned, ref iResumeHandle);
                buffer = Marshal.AllocHGlobal((IntPtr)iBytesNeeded);
                Functions.EnumServicesStatus(_scManager, ServicesTypes.Win32, ServicesState.Stopped | ServicesState.StartPending, buffer, iBytesNeeded, ref iBytesNeeded, ref iServicesReturned, ref iResumeHandle);
                var toReturn = new EnumServiceStatus[iServicesReturned];
                long iPtr = buffer.ToInt64();
                for (int i = 0; i < iServicesReturned; i++)
                {
                    EnumServiceStatus inMemory = (EnumServiceStatus)Marshal.PtrToStructure(new IntPtr(iPtr), typeof(EnumServiceStatus));
                    toReturn[i] = inMemory;
                    iPtr += Marshal.SizeOf(typeof(EnumServiceStatus));
                }
                return toReturn;
            }
            finally
            {
                Marshal.FreeHGlobal(buffer);
            }
        }

        public void StartService(string serviceName)
        {
            var service = Functions.OpenService(_scManager, serviceName, AccessRights.AllAccess);
            if (service == IntPtr.Zero)
                throw new Exception($"OpenService - Code Error: {Functions.GetLastError()}");

            try
            {
                int retCode = Functions.StartService(service, 0, 0);
                if (retCode == 0)
                    throw new Exception($"StartService - Code Error: {Functions.GetLastError()}");
            }
            finally
            {
                Functions.CloseServiceHandle(service);
            }
        }

        public void StopService(string serviceName)
        {
            var service = Functions.OpenService(_scManager, serviceName, AccessRights.AllAccess);
            if (service == IntPtr.Zero)
                throw new Exception($"OpenService - Code Error: {Functions.GetLastError()}");

            try
            {
                var status = new ServiceStatus();
                int retCode = Functions.ControlService(service, ServiceControl.Stop, ref status);
                if (retCode == 0)
                    throw new Exception($"ControlService - Code Error: {Functions.GetLastError()}");
            }
            finally
            {
                Functions.CloseServiceHandle(service);
            }
        }

        public bool WaitForServiceStatus(string serviceName, ServicesState desiredStatus)
        {
            var service = Functions.OpenService(_scManager, serviceName, AccessRights.AllAccess);
            if (service == IntPtr.Zero)
                throw new Exception($"OpenService - Code Error: {Functions.GetLastError()}");

            var status = new ServiceStatus();

            try
            {
                Functions.QueryServiceStatus(service, ref status);
                if (status.dwCurrentState == desiredStatus)
                    return true;

                var actualState = status.dwCurrentState;

                while (status.dwCurrentState == actualState)
                {
                    System.Threading.Thread.Sleep((int)status.dwWaitHint);

                    if (Functions.QueryServiceStatus(service, ref status) == 0)
                        break;
                }
                return (status.dwCurrentState == desiredStatus);
            }
            finally
            {
                Functions.CloseServiceHandle(service);
            }
        }

        public ServicesState GetServiceState(string serviceName)
        {
            var service = Functions.OpenService(_scManager, serviceName, AccessRights.AllAccess);
            if (service == IntPtr.Zero)
                return ServicesState.NotFound;

            try
            {
                var status = new ServiceStatus();
                if (Functions.QueryServiceStatus(service, ref status) == 0)
                    return ServicesState.Unknown;

                return status.dwCurrentState;
            }
            finally
            {
                Functions.CloseServiceHandle(service);
            }
        }
    }
}