using Runner.Business.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Runner.Business.Actions
{
    public partial class ActionControl
    {
        public static ActionControl Build(Run run)
        {
            return new ActionControl(run);
        }
    }
}
