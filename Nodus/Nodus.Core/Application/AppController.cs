using Nodus.Core.Application.Tag;
using Nodus.Core.Model.Application;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nodus.Core.Application
{
    public class AppController
    {
        private static Dictionary<string, AppInstance> _instance;

        static AppController()
        {
            _instance = new Dictionary<string, AppInstance>();
        }

        public static AppInstance Load(string script)
        {
            var domaininfo = new AppDomainSetup();
            domaininfo.ApplicationBase = AppDomain.CurrentDomain.BaseDirectory;
            domaininfo.ShadowCopyFiles = "true";
            var domain = AppDomain.CreateDomain("DynamicDomain", AppDomain.CurrentDomain.Evidence, domaininfo);
            
            var domainInteropType = typeof(DomainInterop);
            var domainInterop = (DomainInterop)domain.CreateInstanceAndUnwrap(domainInteropType.Assembly.FullName, domainInteropType.FullName);
            
            var token = GetFreeToken();
            var app = new AppInstance
            {
                Domain = domain,
                Token = token,
                DomainInterop = domainInterop
            };
            _instance.Add(token, app);

            domainInterop.LoadTags();
            domainInterop.AddScript("root", script);

            return app;
        }

        private static string GetFreeToken()
        {
            string possible = "0123456789ABCDEFGHIJLMNOPRSTUVXZWY";
            string tr = "";
            Random rnd = new Random(DateTime.Now.Millisecond);
            do
            {
                tr = "";
                for (int i = 0; i < 20; i++)
                    tr += possible.Substring(rnd.Next(0, possible.Length), 1);
            } while (_instance.Where(i => i.Key == tr).Count() > 0);
            return tr;
        }

        public static void EndInstance(string token)
        {
            if (!_instance.ContainsKey(token))
                throw new Exception("Token not found!");

            var info = _instance[token];
            info.Dispose();

            _instance.Remove(token);
        }

        public static void EndAllInstance()
        {
            while (_instance.Count() > 0)
            {
                EndInstance(_instance.First().Key);
            }
        }

        public static Result Run(string token)
        {
            if (!_instance.ContainsKey(token))
                throw new Exception("Token not found!");

            var app = _instance[token];

            return app.DomainInterop.Run();
        }
    }
}