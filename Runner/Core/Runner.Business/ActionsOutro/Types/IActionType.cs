using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Runner.Business.ActionsOutro.Types
{
    public interface IActionType
    {
        void Run(ActionCommandContext ctx, );
    }
}
