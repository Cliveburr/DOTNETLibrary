using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RefineryBoard
{
    public class RefineryException : Exception
    {
        public RefineryException()
            : base()
        {
        }

        public RefineryException(string msg, params string[] format)
            : base(string.Format(msg, format))
        {
        }

        public RefineryException(Exception innerException, string msg, params string[] format)
            : base(string.Format(msg, format), innerException)
        {
        }
    }
}