using Nodus.Core.Model.Application;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nodus.Core.Application
{
    public class Context
    {
        public Result Result { get; set; }

        public Context Clone()
        {
            return new Context
            {
                Result = this.Result
            };
        }
    }
}