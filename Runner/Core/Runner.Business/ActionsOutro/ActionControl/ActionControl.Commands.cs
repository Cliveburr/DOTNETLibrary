using Runner.Business.Actions;
using Runner.Business.ActionsOutro.Types;
using Runner.Business.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Runner.Business.ActionsOutro
{
    public partial class ActionControl
    {
        public IEnumerable<CommandEffect> Run(int actionId)
        {
            var action = FindAction(actionId);
            var actionType = FindActionType(action);

            return actionType.Run();
        }

        public IEnumerable<CommandEffect> SetRunning(int actionId)
        {
            var action = FindAction(actionId);
            var actionType = FindActionType(action);

            return actionType.SetRunning();
        }

        public IEnumerable<CommandEffect> SetCompleted(int actionId)
        {
            var action = FindAction(actionId);
            var actionType = FindActionType(action);

            return actionType.SetCompleted();
        }

        public IEnumerable<CommandEffect> SetError(int actionId)
        {
            var action = FindAction(actionId);
            var actionType = FindActionType(action);

            return actionType.SetError();
        }

        public IEnumerable<CommandEffect> Stop(int actionId)
        {
            var action = FindAction(actionId);
            var actionType = FindActionType(action);

            return actionType.Stop();
        }

        public IEnumerable<CommandEffect> SetStopped(int actionId)
        {
            var action = FindAction(actionId);
            var actionType = FindActionType(action);

            return actionType.SetStopped();
        }

        public IEnumerable<CommandEffect> SetBreakPoint(int actionId)
        {
            var action = FindAction(actionId);
            var actionType = FindActionType(action);

            return actionType.SetBreakPoint();
        }

        public IEnumerable<CommandEffect> CleanBreakPoint(int actionId)
        {
            var action = FindAction(actionId);
            var actionType = FindActionType(action);

            return actionType.CleanBreakPoint();
        }
    }
}
