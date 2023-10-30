using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Runner.Business.Actions
{
    public class CommandEffect
    {
        public ComandEffectType Type { get; private set; }
        public Action? Action { get; private set; }
        public ActionContainer? ActionContainer { get; private set; }

        public CommandEffect(ComandEffectType type, Action action)
        {
            Type = type;
            Action = action;
        }

        public CommandEffect(ComandEffectType type, ActionContainer actionContainer)
        {
            Type = type;
            ActionContainer = actionContainer;
        }

        public CommandEffect(ComandEffectType type, ActionContainer actionContainer, Action action)
        {
            Type = type;
            ActionContainer = actionContainer;
            Action = action;
        }
    }

    public enum ComandEffectType
    {
        ActionUpdateStatus = 0,
        ActionContainerUpdatePosition = 1,
        ActionContainerCreateJobToRun = 2,
        ActionContainerUpdateStatus = 3,
        ActionContainerUpdatePositionAndStatus = 4
    }
}
