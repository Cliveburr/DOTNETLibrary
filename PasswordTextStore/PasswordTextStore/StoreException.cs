using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PasswordTextStore
{
    public class StoreException : Exception
    {
        public bool Terminate { get; set; }

        public StoreException()
            : base()
        {
        }

        public StoreException(string msg, params string[] format)
            : base(string.Format(msg, format))
        {
        }

        public StoreException(bool terminate, string msg, params string[] format)
            : base(string.Format(msg, format))
        {
            Terminate = terminate;
        }

        //public StoreException(Exception innerException, string msg, params string[] format)
        //    : base(string.Format(msg, format), innerException)
        //{
        //}
    }
}