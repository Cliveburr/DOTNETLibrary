using Runner.Business.ActionsOutro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Test = Microsoft.VisualStudio.TestTools.UnitTesting.Assert;
using Action = Runner.Business.Actions.Action;


namespace Runner.Business.Tests.Helpers
{
    public abstract class TestActionsBase
    {
        protected abstract ActionControl GetControl();

        private ActionControl? _control;

        private ActionControl Control
        {
            get
            {
                if (_control == null)
                {
                    _control = GetControl();
                }
                return _control;
            }
        }

        protected TestActionResults Run(string actionLabel)
        {
            var action = Control.FindAction(actionLabel);
            Test.IsNotNull(action);
            return new TestActionResults(Control.Run(action.ActionId));
        }

        protected TestActionResults SetRunning(string actionLabel)
        {
            var action = Control.FindAction(actionLabel);
            Test.IsNotNull(action);
            return new TestActionResults(Control.SetRunning(action.ActionId));
        }

        protected TestActionResults SetCompleted(string actionLabel)
        {
            var action = Control.FindAction(actionLabel);
            Test.IsNotNull(action);
            return new TestActionResults(Control.SetCompleted(action.ActionId));
        }

        protected TestActionResults SetError(string actionLabel)
        {
            var action = Control.FindAction(actionLabel);
            Test.IsNotNull(action);
            return new TestActionResults(Control.SetError(action.ActionId));
        }

        protected TestActionResults Stop(string actionLabel)
        {
            var action = Control.FindAction(actionLabel);
            Test.IsNotNull(action);
            return new TestActionResults(Control.Stop(action.ActionId));
        }

        protected TestActionResults SetStopped(string actionLabel)
        {
            var action = Control.FindAction(actionLabel);
            Test.IsNotNull(action);
            return new TestActionResults(Control.SetStopped(action.ActionId));
        }

        protected TestActionResults SetBreakPoint(string actionLabel)
        {
            var action = Control.FindAction(actionLabel);
            Test.IsNotNull(action);
            return new TestActionResults(Control.SetBreakPoint(action.ActionId));
        }

        protected TestActionResults CleanBreakPoint(string actionLabel)
        {
            var action = Control.FindAction(actionLabel);
            Test.IsNotNull(action);
            return new TestActionResults(Control.CleanBreakPoint(action.ActionId));
        }

        protected void CheckAllCompleted()
        {
            var otherThanCompleted = Control.EntityRun.Actions
                .Where(a => a.Status != ActionStatus.Completed)
                .Any();
            Test.IsFalse(otherThanCompleted);
        }
    }
}
