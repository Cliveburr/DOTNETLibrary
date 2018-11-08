using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nodus.Core.Tokens
{
    public class TokenStorage<T>
    {
        public string Token { get; set; }
        public T Value { get; set; }
    }
}