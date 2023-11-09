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

            public TestResults(List<CommandEffect> effects)
            {
                _effects = effects;
                _sequence = 0;
            }

            public TestResults TestCount(int count)
            {
                Test.AreEqual(_effects.Count, count);
                return this;
            }

            public TestResults TestInSequeceActionUpdateStatus(ActionStatus actionStatus)
            {
                Test.AreEqual(_effects[_sequence].Action.Status, actionStatus);
                Test.AreEqual(_effects[_sequence].Type, ComandEffectType.ActionUpdateStatus);
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

        //protected void SetRunning(ActionControl control, string actionLabel)
        //{
        //    var actionContainer = control.Run.Containers
        //        .FirstOrDefault(c => c.Label == actionLabel);
        //    Test.IsNotNull(actionContainer);
        //    SetRunning(control, actionContainer.ActionContainerId);
        //}

        //protected void SetRunningEmptyContainer(ActionControl control, int actionContainerId)
        //{
        //    var effects = control.SetCompleted(actionContainerId);

        //    Test.AreEqual(effects[0].Type, ComandEffectType.ActionContainerUpdateStatus);
        //    var container = effects[0].ActionContainer!;
        //    Test.AreEqual(container.Status, ActionContainerStatus.Completed);

        //    Test.AreEqual(effects.Count, 1 + (container.Next.Count * 2));

        //    for (var i = 1; i < 1 + container.Next.Count; i += 2)
        //    {
        //        Test.AreEqual(effects[i].Type, ComandEffectType.ActionContainerUpdatePositionAndStatus);
        //        Test.AreEqual(effects[i].ActionContainer!.Position, 0);
        //        Test.AreEqual(effects[i].ActionContainer!.Status, ActionContainerStatus.Ready);

        //        Test.AreEqual(effects[i + 1].Type, ComandEffectType.ActionContainerCreateJobToRun);
        //    }
        //}

        //protected void SetRunningEmptyContainer(ActionControl control, string actionLabel)
        //{
        //    var actionContainer = control.Run.Containers
        //        .FirstOrDefault(c => c.Label == actionLabel);
        //    Test.IsNotNull(actionContainer);
        //    SetRunningEmptyContainer(control, actionContainer.ActionContainerId);
        //}

        //protected void SetError(ActionControl control, int actionContainerId)
        //{
        //    var effects = control.SetError(actionContainerId);
        //    Test.AreEqual(effects.Count, 1);

        //    Test.AreEqual(effects[0].Action!.Status, ActionStatus.Error);
        //    Test.AreEqual(effects[0].Type, ComandEffectType.ActionUpdateStatus);
        //}

        //protected void SetError(ActionControl control, string actionLabel)
        //{
        //    var actionContainer = control.Run.Containers
        //        .FirstOrDefault(c => c.Label == actionLabel);
        //    Test.IsNotNull(actionContainer);
        //    SetError(control, actionContainer.ActionContainerId);
        //}

        //protected void SetCompletedOnSameContainer(ActionControl control, int actionContainerId)
        //{
        //    var effects = control.SetCompleted(actionContainerId);
        //    Test.AreEqual(effects.Count, 3);

        //    Test.AreEqual(effects[0].Action!.Status, ActionStatus.Completed);
        //    Test.AreEqual(effects[0].Type, ComandEffectType.ActionUpdateStatus);

        //    Test.AreEqual(effects[1].Type, ComandEffectType.ActionContainerUpdatePosition);

        //    Test.AreEqual(effects[2].Type, ComandEffectType.ActionContainerCreateJobToRun);
        //}

        //protected void SetCompletedOnSameContainer(ActionControl control, string actionLabel)
        //{
        //    var actionContainer = control.Run.Containers
        //        .FirstOrDefault(c => c.Label == actionLabel);
        //    Test.IsNotNull(actionContainer);
        //    SetCompletedOnSameContainer(control, actionContainer.ActionContainerId);
        //}

        //protected void SetCompletedAndBreak(ActionControl control, int actionContainerId)
        //{
        //    var effects = control.SetCompleted(actionContainerId);
        //    Test.AreEqual(effects.Count, 2);

        //    Test.AreEqual(effects[0].Action!.Status, ActionStatus.Completed);
        //    Test.AreEqual(effects[0].Type, ComandEffectType.ActionUpdateStatus);

        //    Test.AreEqual(effects[1].Type, ComandEffectType.ActionContainerUpdatePosition);
        //}

        //protected void SetCompletedAndBreak(ActionControl control, string actionLabel)
        //{
        //    var actionContainer = control.Run.Containers
        //        .FirstOrDefault(c => c.Label == actionLabel);
        //    Test.IsNotNull(actionContainer);
        //    SetCompletedAndBreak(control, actionContainer.ActionContainerId);
        //}

        //protected void SetCompletedAndMoveToNextContainer(ActionControl control, int actionContainerId)
        //{
        //    var effects = control.SetCompleted(actionContainerId);

        //    Test.AreEqual(effects[0].Action!.Status, ActionStatus.Completed);
        //    Test.AreEqual(effects[0].Type, ComandEffectType.ActionUpdateStatus);

        //    Test.AreEqual(effects[1].Type, ComandEffectType.ActionContainerUpdateStatus);
        //    var container = effects[1].ActionContainer!;
        //    Test.AreEqual(container.Status, ActionContainerStatus.Completed);

        //    Test.AreEqual(effects.Count, 2 + (container.Next.Count * 2));//

        //    for (var i = 2; i < 2 + container.Next.Count; i += 2)
        //    {
        //        Test.AreEqual(effects[i].Type, ComandEffectType.ActionContainerUpdatePositionAndStatus);
        //        Test.AreEqual(effects[i].ActionContainer!.Position, 0);
        //        Test.AreEqual(effects[i].ActionContainer!.Status, ActionContainerStatus.Ready);

        //        Test.AreEqual(effects[i + 1].Type, ComandEffectType.ActionContainerCreateJobToRun);
        //    }
        //}

        //protected void SetCompletedAndMoveToNextContainer(ActionControl control, string actionLabel)
        //{
        //    var actionContainer = control.Run.Containers
        //        .FirstOrDefault(c => c.Label == actionLabel);
        //    Test.IsNotNull(actionContainer);
        //    SetCompletedAndMoveToNextContainer(control, actionContainer.ActionContainerId);
        //}

        //protected void SetCompletedAndDone(ActionControl control, int actionContainerId)
        //{
        //    var effects = control.SetCompleted(actionContainerId);
        //    Test.AreEqual(effects.Count, 2);

        //    Test.AreEqual(effects[0].Action!.Status, ActionStatus.Completed);
        //    Test.AreEqual(effects[0].Type, ComandEffectType.ActionUpdateStatus);

        //    Test.AreEqual(effects[1].Type, ComandEffectType.ActionContainerUpdateStatus);
        //    Test.AreEqual(effects[1].ActionContainer!.Status, ActionContainerStatus.Completed);
        //}

        //protected void SetCompletedAndDone(ActionControl control, string actionLabel)
        //{
        //    var actionContainer = control.Run.Containers
        //        .FirstOrDefault(c => c.Label == actionLabel);
        //    Test.IsNotNull(actionContainer);
        //    SetCompletedAndDone(control, actionContainer.ActionContainerId);
        //}

        //protected void SetCompletedAndMoveToNextContainerWithBreak1(ActionControl control, int actionContainerId)
        //{
        //    var effects = control.SetCompleted(actionContainerId);
        //    Test.AreEqual(effects.Count, 3);

        //    Test.AreEqual(effects[0].Action!.Status, ActionStatus.Completed);
        //    Test.AreEqual(effects[0].Type, ComandEffectType.ActionUpdateStatus);

        //    Test.AreEqual(effects[1].Type, ComandEffectType.ActionContainerUpdateStatus);
        //    Test.AreEqual(effects[1].ActionContainer!.Status, ActionContainerStatus.Completed);

        //    Test.AreEqual(effects[2].Type, ComandEffectType.ActionContainerUpdatePositionAndStatus);
        //    Test.AreEqual(effects[2].ActionContainer!.Position, 0);
        //    Test.AreEqual(effects[2].ActionContainer!.Status, ActionContainerStatus.Ready);
        //}

        //protected void SetCompletedAndMoveToNextContainerWithBreak1(ActionControl control, string actionLabel)
        //{
        //    var actionContainer = control.Run.Containers
        //        .FirstOrDefault(c => c.Label == actionLabel);
        //    Test.IsNotNull(actionContainer);
        //    SetCompletedAndMoveToNextContainerWithBreak1(control, actionContainer.ActionContainerId);
        //}
    }
}
