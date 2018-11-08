using Nodus.Core.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nodus.Test
{
    static class Program
    {

        static void Main()
        {
            //var nodus = new NodusClient("sbrraobld02", 60081);
            var nodus = new NodusClient("localhost", 60081);
            nodus.Start();

            var ping = nodus.Ping();
            var version = nodus.Version();

            nodus.RouteTo("sbrraobld02", 60081);

            var ping2 = nodus.Ping();
            var version2 = nodus.Version();

            nodus.Update(@"D:\Visual Studio Stuffs\Nodus\Nodus.Service\bin\Debug");

            var ping3 = nodus.Ping();
            var version3 = nodus.Version();


            //nodus.IO.CreateDirectory(@"D:\Nodus\something");
            //var empty = nodus.IO.EmptyDirectory(@"D:\Nodus");
            //var delete = nodus.IO.DeleteDirectory(@"D:\Nodus");

            //nodus.SynchronizeFile(@"D:\Nodus\source\01 Moonchild.wma", @"D:\Nodus\target");

            //var serviceFolder = @"D:\Visual Studio Stuffs\Nodus\Nodus.Service\bin\Debug";
            //var destineFolder = @"D:\Nodus\target";

            //nodus.SynchronizeFolder(serviceFolder, destineFolder);
        }
    }
}