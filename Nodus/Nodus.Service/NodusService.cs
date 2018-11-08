using Nodus.Core.Host;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace Nodus.Service
{
    public partial class NodusService : ServiceBase
    {
        private NodusHost _nodus;

        public NodusService()
        {
            InitializeComponent();
        }

        public void InteractiveStart()
        {
            OnStart(null);
        }

        public void InteractiveStop()
        {
            OnStop();
        }

        protected override void OnStart(string[] args)
        {
            var section = ConfigurationManager.GetSection("NodusService") as NameValueCollection;

            var portStr = section["Port"];
            if (portStr == null)
                throw new Exception(@"Error loading key ""Port"" of section ""NodusService"" on config file!");

            var port = -1;
            if (!int.TryParse(portStr, out port))
                throw new Exception($@"Invalid value ""Port"" of section ""NodusService"" on config file! Value = {portStr}");

            _nodus = new NodusHost(port);
            _nodus.OnClose += _nodus_OnClose;
            _nodus.Start();
        }

        private void _nodus_OnClose()
        {
            _nodus = null;
            Stop();
        }

        protected override void OnStop()
        {
            _nodus?.Close();
        }
    }
}
