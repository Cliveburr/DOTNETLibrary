using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Runner.Business.ActionsOutro
{
    public class CommandEffect
    {
        public ComandEffectType Type { get; }
        public Action Action { get; }

        public CommandEffect(ComandEffectType type, Action action)
        {
            Type = type;
            Action = action;
        }
    }

    public enum ComandEffectType
    {
        ActionUpdateStatus = 0,
        ActionUpdateToRun = 1,
        ActionUpdateWithCursor = 2,
        ActionUpdateBreakPoint = 3
    }
}
