using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Runner.Business.Actions
{
    public record CommandContext(ActionControl Control, List<CommandEffect> Effects);
}
