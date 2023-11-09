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
        public List<CommandEffect> Run(int actionId)
        {
            var action = FindAction(actionId);

            var actionType = FindActionType(action);

            return actionType.Run();
        }

        public List<CommandEffect> SetRunning(int actionId)
        {
            var action = FindAction(actionId);

            var actionType = FindActionType(action);

            return actionType.SetRunning();
        }

        public List<CommandEffect> SetCompleted(int actionId)
        {
            var action = FindAction(actionId);

            var actionType = FindActionType(action);

            return actionType.SetCompleted();
        }

        public List<CommandEffect> SetError(int actionId)
        {
            var action = FindAction(actionId);

            var actionType = FindActionType(action);

            return actionType.SetError();
        }

        public List<CommandEffect> Stop(int actionId)
        {
            var action = FindAction(actionId);

            var actionType = FindActionType(action);

            return actionType.Stop();
        }

        public List<CommandEffect> SetStopped(int actionId)
        {
            var action = FindAction(actionId);

            var actionType = FindActionType(action);

            return actionType.SetStopped();
        }
    }
}
