using Runner.Business.ActionsOutro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Test = Microsoft.VisualStudio.TestTools.UnitTesting.Assert;
using Action = Runner.Business.Actions.Action;


namespace Runner.Business.Tests.Actions
{
    public abstract class TestActionsBase2
    {
        protected class TestResults
        {
            private readonly List<CommandEffect> _effects;
            private int _sequence;

            public TestResults(IEnumerable<CommandEffect> effects)
            {
                _effects = effects.ToList();
                _sequence = 0;
            }

            public TestResults TestCount(int count)
            {
                Test.AreEqual(_effects.Count, count);
                return this;
            }

            public TestResults TestInSequeceActionUpdateStatus(ActionStatus actionStatus)
            {
                Test.IsNotNull(_effects[_sequence].Action);
                Test.AreEqual(_effects[_sequence].Action!.Status, actionStatus);
                Test.AreEqual(_effects[_sequence].Type, ComandEffectType.ActionUpdateStatus);
                _sequence++;
                return this;
            }

            public TestResults TestInSequeceActionUpdateToRun()
            {
                Test.IsNotNull(_effects[_sequence].Action);
                Test.AreEqual(_effects[_sequence].Action!.Status, ActionStatus.ToRun);
                Test.AreEqual(_effects[_sequence].Type, ComandEffectType.ActionUpdateToRun);
                _sequence++;
                return this;
            }

            public TestResults TestInSequeceActionUpdateBreakPoint(bool value)
            {
                Test.IsNotNull(_effects[_sequence].Action);
                Test.AreEqual(_effects[_sequence].Action!.BreakPoint, value);
                Test.AreEqual(_effects[_sequence].Type, ComandEffectType.ActionUpdateBreakPoint);
                _sequence++;
                return this;
            }

            public TestResults TestInSequeceCursorUpdate()
            {
                Test.IsNotNull(_effects[_sequence].Cursor);
                Test.AreEqual(_effects[_sequence].Type, ComandEffectType.CursorUpdate);
                _sequence++;
                return this;
            }
        }

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

        protected TestResults Run(string actionLabel)
        {
            var action = Control.FindAction(actionLabel);
            Test.IsNotNull(action);
            return new TestResults(Control.Run(action.ActionId));
        }

        protected TestResults SetRunning(string actionLabel)
        {
            var action = Control.FindAction(actionLabel);
            Test.IsNotNull(action);
            return new TestResults(Control.SetRunning(action.ActionId));
        }

        protected TestResults SetCompleted(string actionLabel)
        {
            var action = Control.FindAction(actionLabel);
            Test.IsNotNull(action);
            return new TestResults(Control.SetCompleted(action.ActionId));
        }


        protected TestResults SetError(string actionLabel)
        {
            var action = Control.FindAction(actionLabel);
            Test.IsNotNull(action);
            return new TestResults(Control.SetError(action.ActionId));
        }

        protected TestResults Stop(string actionLabel)
        {
            var action = Control.FindAction(actionLabel);
            Test.IsNotNull(action);
            return new TestResults(Control.Stop(action.ActionId));
        }

        protected TestResults SetStopped(string actionLabel)
        {
            var action = Control.FindAction(actionLabel);
            Test.IsNotNull(action);
            return new TestResults(Control.SetStopped(action.ActionId));
        }

        protected TestResults SetBreakPoint(string actionLabel)
        {
            var action = Control.FindAction(actionLabel);
            Test.IsNotNull(action);
            return new TestResults(Control.SetBreakPoint(action.ActionId));
        }

        protected TestResults CleanBreakPoint(string actionLabel)
        {
            var action = Control.FindAction(actionLabel);
            Test.IsNotNull(action);
            return new TestResults(Control.CleanBreakPoint(action.ActionId));
        }
    }
}
