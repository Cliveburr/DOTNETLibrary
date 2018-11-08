using Nodus.Core.Model.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nodus.Core.Client
{
    public partial class NodusClient
    {
        public bool Ping()
        {
            return _core.Ping();
        }

        public Version Version()
        {
            return _core.Version();
        }

        public Hoop RouteTo(string host, int port)
        {
            _core.RouteTo(host, port);
            _IO.RouteTo(host, port);
            _application.RouteTo(host, port);

            var hoop = new Hoop
            {
                Index = Hoops.Count,
                Host = host,
                Port = port
            };
            Hoops.Add(hoop);

            return hoop;
        }

        public void Update(string localNewFiles)
        {
            _IO.SynchronizeFolder(0, localNewFiles, @"%ROOTPATH%\\Update", true, null, true, null);
            try { _core.Update(); } catch { };
            try { _core.Close(); } catch { };
            try { _IO.Close(); } catch { };
            try { _application.Close(); } catch { };

            System.Threading.Thread.Sleep(2000);
            var timeout = DateTime.Now.AddMilliseconds(60000);
            while (DateTime.Now < timeout)
            {
                try
                {
                    Start();
                    _core.Ping();

                    for (var i = 1; i < Hoops.Count; i++)
                    {
                        _core.RouteTo(Hoops[i].Host, Hoops[i].Port);
                        _IO.RouteTo(Hoops[i].Host, Hoops[i].Port);
                        _application.RouteTo(Hoops[i].Host, Hoops[i].Port);
                    }

                    break;
                }
                catch
                {
                    if (_core != null)
                    {
                        try { _core.Close(); } catch { };
                        _core = null;
                    }
                    if (_IO != null)
                    {
                        try { _IO.Close(); } catch { };
                        _IO = null;
                    }
                    if (_application != null)
                    {
                        try { _application.Close(); } catch { };
                        _application = null;
                    }
                    System.Threading.Thread.Sleep(400);
                }
            }

            if (_core == null)
                throw new Exception("Can't reconect to the service!");
        }
    }
}