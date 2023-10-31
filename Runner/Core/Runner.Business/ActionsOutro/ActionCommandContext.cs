using Runner.Business.ActionsOutro.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Runner.Business.ActionsOutro
{
    public class ActionCommandContext
    {
        public required List<Action> Actions { get; set; }
        public required List<Cursor> Cursors { get; set; }
        public required IActionType Root { get; set; }
    }
}
