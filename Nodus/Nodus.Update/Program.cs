using Nodus.Update.WindowsApi;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nodus.Update
{
    class Program
    {
        const string ServiceName = "NodusService";

        static void Main(string[] args)
        {
            System.Threading.Thread.Sleep(300);

            try
            {
                using (var services = new Services())
                {
                    if (!services.GetAllServices()
                        .Where(s => s.pServiceName == ServiceName)
                        .Any())
                        throw new Exception("Service not installed!");

                    try { services.StopService(ServiceName); } catch { }

                    if (!services.WaitForServiceStatus(ServiceName, ServicesState.Stopped))
                        throw new Exception("Can't stop the service!");

                    System.Threading.Thread.Sleep(500);
                    CopyMe();
                    System.Threading.Thread.Sleep(500);

                    var tries = 3;
                    while (tries-- > 0)
                    {
                        try { services.StartService(ServiceName); } catch { }
                        if (services.WaitForServiceStatus(ServiceName, ServicesState.Running))
                            break;
                        System.Threading.Thread.Sleep(500);
                    }

                    if (services.GetServiceState(ServiceName) != ServicesState.Running)
                        throw new Exception("Can't start the service!");
                }
            }
            catch (Exception err)
            {
                var dumpFile = Path.Combine(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location), "Error.txt");
                File.WriteAllText(dumpFile, err.ToString());
            }
            Environment.Exit(0);
        }

        static void Log(string msg)
        {
            var logFile = Path.Combine(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location), "Log.txt");
            File.AppendAllText(logFile, msg + Environment.NewLine);
        }

        static void CopyMe()
        {
            var root = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            var servicePath = Path.GetDirectoryName(root);

            var files = Directory.EnumerateFiles(root, "*", SearchOption.TopDirectoryOnly);
            foreach (var file in files)
            {
                var fileName = Path.GetFileName(file);
                var servicePathFile = Path.Combine(servicePath, fileName);

                File.Copy(file, servicePathFile, true);
            }
        }
    }
}