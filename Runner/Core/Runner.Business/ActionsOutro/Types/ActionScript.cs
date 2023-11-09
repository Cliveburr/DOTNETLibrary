using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Runner.Business.ActionsOutro.Types
{
    public class ActionScript : ActionTypesBase
    {
        public ActionScript(ActionControl control, Action action)
            : base(control, action)
        {
        }

        public override List<CommandEffect> Run()
        {
            _ = _control.FindCursor(_action.ActionId); // just to confirm cursor on it

            Assert.Enum.In(_action.Status, new[] {
                ActionStatus.Waiting,
                ActionStatus.Stopped,
                ActionStatus.Error
            }, $"Action in wrong status to Run! {_action.ActionId}-{_action.Label}");

            _action.Status = ActionStatus.ToRun;

            var tr = new List<CommandEffect>
            {
                new CommandEffect(ComandEffectType.ActionUpdateStatus, _action)
            };

            if (_action.Parent.HasValue)
            {
                tr.AddRange(PropagateBackRun(_action.Parent.Value));
            }

            return tr;
        }

        public override List<CommandEffect> BackRun()
        {
            throw new RunnerException("ActionScript shound't never run from back!");
        }

        public override List<CommandEffect> SetRunning()
        {
            _ = _control.FindCursor(_action.ActionId); // just to confirm cursor on it

            Assert.Enum.In(_action.Status, new[] {
                ActionStatus.ToRun
            }, $"Action in wrong status to Running! {_action.ActionId}-{_action.Label}");

            _action.Status = ActionStatus.Running;

            var tr = new List<CommandEffect>
            {
                new CommandEffect(ComandEffectType.ActionUpdateStatus, _action)
            };

            if (_action.Parent.HasValue)
            {
                tr.AddRange(PropagateBackSetRunning(_action.Parent.Value));
            }

            return tr;
        }

        public override List<CommandEffect> BackSetRunning()
        {
            throw new RunnerException("ActionScript shound't never running from back!");
        }

        public override List<CommandEffect> SetCompleted()
        {
            _ = _control.FindCursor(_action.ActionId); // just to confirm cursor on it

            Assert.Enum.In(_action.Status, new[] {
                ActionStatus.Running
            }, $"Action in wrong status to Completed! {_action.ActionId}-{_action.Label}");

            _action.Status = ActionStatus.Completed;

            var tr = new List<CommandEffect>
            {
                new CommandEffect(ComandEffectType.ActionUpdateStatus, _action)
            };

            if (_action.Parent.HasValue)
            {
                tr.AddRange(PropagateBackSetCompleted(_action.Parent.Value));
            }

            return tr;
        }

        public override List<CommandEffect> BackSetCompleted()
        {
            throw new RunnerException("ActionScript shound't never completed from back!");
        }

        public override List<CommandEffect> SetError()
        {
            _ = _control.FindCursor(_action.ActionId); // just to confirm cursor on it

            Assert.Enum.In(_action.Status, new[] {
                ActionStatus.Running
            }, $"Action in wrong status to Completed! {_action.ActionId}-{_action.Label}");

            _action.Status = ActionStatus.Error;

            var tr = new List<CommandEffect>
            {
                new CommandEffect(ComandEffectType.ActionUpdateStatus, _action)
            };

            if (_action.Parent.HasValue)
            {
                tr.AddRange(PropagateBackSetError(_action.Parent.Value));
            }

            return tr;
        }

        public override List<CommandEffect> BackSetError()
        {
            throw new RunnerException("ActionScript shound't never error from back!");
        }

        public override List<CommandEffect> Stop()
        {
            _ = _control.FindCursor(_action.ActionId); // just to confirm cursor on it

            Assert.Enum.In(_action.Status, new[] {
                ActionStatus.Running
            }, $"Action in wrong status to Completed! {_action.ActionId}-{_action.Label}");

            _action.Status = ActionStatus.ToStop;

            var tr = new List<CommandEffect>
            {
                new CommandEffect(ComandEffectType.ActionUpdateStatus, _action)
            };

            if (_action.Parent.HasValue)
            {
                tr.AddRange(PropagateBackStop(_action.Parent.Value));
            }

            return tr;
        }

        public override List<CommandEffect> BackStop()
        {
            throw new RunnerException("ActionScript shound't never stop from back!");
        }

        public override List<CommandEffect> SetStopped()
        {
            _ = _control.FindCursor(_action.ActionId); // just to confirm cursor on it

            Assert.Enum.In(_action.Status, new[] {
                ActionStatus.Running
            }, $"Action in wrong status to Completed! {_action.ActionId}-{_action.Label}");

            _action.Status = ActionStatus.ToStop;

            var tr = new List<CommandEffect>
            {
                new CommandEffect(ComandEffectType.ActionUpdateStatus, _action)
            };

            if (_action.Parent.HasValue)
            {
                tr.AddRange(PropagateBackSetStoppped(_action.Parent.Value));
            }

            return tr;
        }

        public override List<CommandEffect> BackSetStopped()
        {
            throw new RunnerException("ActionScript shound't never stopped from back!");
        }
    }
}
