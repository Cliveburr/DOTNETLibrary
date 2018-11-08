using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Nodus.Service
{
    static class Program
    {
        public static NodusService Service { get; private set; }
        private static Timer _toClear;

        static void Main()
        {
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;

            Service = new NodusService();
            if (Environment.UserInteractive)
            {
                Service.InteractiveStart();
                Console.WriteLine("Service running. Press any key to exit.");
                Console.ReadKey(true);
                Service.InteractiveStop();
            }
            else
            {
                _toClear = new Timer(ClearUpdate, null, 2000, Timeout.Infinite);
                ServiceBase.Run(Service);
            }
        }

        private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            var ex = (Exception)e.ExceptionObject;

            var errorFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Errors");

            if (!Directory.Exists(errorFolder))
                Directory.CreateDirectory(errorFolder);

            string errorFile = "";
            do
            {
                errorFile = Path.Combine(errorFolder, string.Format(@"Err_{0}.txt", new Random(DateTime.Now.Millisecond).Next(0, 99999).ToString()));
            } while (File.Exists(errorFile));

            var log = new StringBuilder();
            log.AppendLine(new String('#', 100));
            log.AppendLine(new String('#', 100));
            log.AppendLine();
            log.AppendFormat("Date: {0}\r\n", DateTime.Now.ToString());

            var thisEx = ex;
            while (thisEx != null)
            {
                log.AppendLine();
                log.AppendFormat("Message: {0}\r\n", ex.Message);
                log.AppendFormat("Error: {0}\r\n", ex.StackTrace);
                log.AppendLine();

                var stack = new StackTrace(thisEx, true);
                foreach (var frame in stack.GetFrames())
                {
                    log.AppendFormat("Frame: {0}", frame.ToString());
                }

                thisEx = thisEx.InnerException;
            }

            log.AppendLine();
            log.AppendLine(new String('#', 100));
            log.AppendLine(new String('#', 100));

            File.AppendAllText(errorFile, log.ToString());
        }

        static void ClearUpdate(object state)
        {
            _toClear.Dispose();
            var updatePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Update");
            try
            {
                Directory.Delete(updatePath, true);
            }
            catch
            {
                Thread.Sleep(1000);
                try
                {
                    Directory.Delete(updatePath, true);
                }
                catch
                {
                    Thread.Sleep(1000);
                    try
                    {
                        Directory.Delete(updatePath, true);
                    }
                    catch
                    {
                    }
                }
            }
        }
    }
}