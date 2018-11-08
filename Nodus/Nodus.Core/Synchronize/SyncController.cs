using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nodus.Core.Synchronize
{
    public class SyncController : IDisposable
    {
        public ConcurrentBag<SyncItem> Items { get; private set; }

        public SyncController()
        {
            Items = new ConcurrentBag<Synchronize.SyncItem>();
        }

        public string GetFreeToken()
        {
            string possible = "0123456789ABCDEFGHIJLMNOPRSTUVXZWY";
            string tr = "";
            Random rnd = new Random(DateTime.Now.Millisecond);
            do
            {
                tr = "";
                for (int i = 0; i < 20; i++)
                    tr += possible.Substring(rnd.Next(0, possible.Length), 1);
            } while (Items.Where(i => i.Token == tr).Count() > 0);
            return tr;
        }

        public SyncItem GetItem(string token)
        {
            var tr = Items.Where(i => i.Token == token).FirstOrDefault();
            if (tr == null)
                throw new ServiceFault("Token not found!");
            return tr;
        }

        public void Dispose()
        {
            foreach (var item in Items)
            {
                item.File.Close();
            }
        }
    }
}