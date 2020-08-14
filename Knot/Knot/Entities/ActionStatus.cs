using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Knot.Entities
{
    public enum ActionStatus
    {
		Waiting = 0,                // ação em espera
		Cursor = 1,                 // ação pronta para ser executada
		Queued = 2,                 // ação enfileirada para ser executada
		Ready = 3,                  // ação marcada para ser exetuda
		Running = 4,                // ação sendo executada
		Stopping = 5,               // ação agendada para parar
		Error = 6,                  // ação ou algum filho com erro
		Completed = 7,              // ação totalmente completada
		Invalidated = 8             // ação e seus filhos invalidados
	}
}