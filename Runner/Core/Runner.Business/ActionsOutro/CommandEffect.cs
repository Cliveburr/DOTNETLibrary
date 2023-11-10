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
        public Action? Action { get; }
        public Cursor? Cursor { get; }

        public CommandEffect(ComandEffectType type, Action action)
        {
            Type = type;
            Action = action;
        }

        public CommandEffect(ComandEffectType type, Cursor cursor)
        {
            Type = type;
            Cursor = cursor;
        }
    }

    public enum ComandEffectType
    {
        ActionUpdateStatus = 0,
        ActionUpdateToRun = 1,
        CursorUpdate = 2,
        ActionUpdateBreakPoint = 3
    }
}
