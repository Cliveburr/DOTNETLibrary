using Nodus.Core.Model.Application;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nodus.Core.Application2
{
    public class AppRunner
    {
        public Result Run(string scriptFile, string function, params object[] arguments)
        {
            var domaininfo = new AppDomainSetup();
            domaininfo.ApplicationBase = AppDomain.CurrentDomain.BaseDirectory;
            domaininfo.ShadowCopyFiles = "true";
            var domain = AppDomain.CreateDomain("DynamicDomain", AppDomain.CurrentDomain.Evidence, domaininfo);

            var domainInteropType = typeof(DomainInterop);
            var domainInterop = (DomainInterop)domain.CreateInstanceAndUnwrap(domainInteropType.Assembly.FullName, domainInteropType.FullName);

            var memoryStream = new MemoryStream();
            var streamWrite = new StreamWriter(memoryStream);
            domainInterop.SetConsoleOut(streamWrite);
            domainInterop.LoadAssembly(scriptFile);
            domainInterop.Run(function, arguments);

            streamWrite.Flush();
            var consoleOut = Encoding.UTF8.GetString(memoryStream.ToArray());

            AppDomain.Unload(domain);

            return new Result
            {
                Messager = consoleOut
            };
        }
    }
}