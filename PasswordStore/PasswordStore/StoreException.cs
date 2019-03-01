using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PasswordStore
{
    public class StoreException : Exception
    {
        public StoreException()
            : base()
        {
        }

        public StoreException(string msg, params string[] format)
            : base(string.Format(msg, format))
        {
        }

        public StoreException(Exception innerException, string msg, params string[] format)
            : base(string.Format(msg, format), innerException)
        {
        }
    }
}