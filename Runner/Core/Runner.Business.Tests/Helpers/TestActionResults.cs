using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Test = Microsoft.VisualStudio.TestTools.UnitTesting.Assert;
using Action = Runner.Business.Actions.Action;
using Runner.Business.ActionsOutro;

namespace Runner.Business.Tests.Helpers
{
    public class TestActionResults
    {
        private readonly List<CommandEffect> _effects;

        public TestActionResults(IEnumerable<CommandEffect> effects)
        {
            _effects = effects.ToList();
        }

        public void IsCheckedAll()
        {
            Test.AreEqual(_effects.Count, 0);
        }

        public TestActionResults HasActionUpdateToRun(string actionLabel)
        {
            var action = _effects
                .FirstOrDefault(e =>
                    e.Action.Label == actionLabel
                    && e.Type == ComandEffectType.ActionUpdateToRun
                    && e.Action.Status == ActionStatus.ToRun);
            Test.IsNotNull(action);
            _effects.Remove(action);
            return this;
        }

        public TestActionResults HasActionUpdateRunning(string actionLabel)
        {
            var action = _effects
                .FirstOrDefault(e =>
                    e.Action.Label == actionLabel
                    && e.Type == ComandEffectType.ActionUpdateStatus
                    && e.Action.Status == ActionStatus.Running);
            Test.IsNotNull(action);
            _effects.Remove(action);
            return this;
        }

        public TestActionResults HasActionUpdateCompleted(string actionLabel)
        {
            var action = _effects
                .FirstOrDefault(e =>
                    e.Action.Label == actionLabel
                    && e.Type == ComandEffectType.ActionUpdateStatus
                    && e.Action.Status == ActionStatus.Completed);
            Test.IsNotNull(action);
            _effects.Remove(action);
            return this;
        }

        public TestActionResults HasActionUpdateError(string actionLabel)
        {
            var action = _effects
                .FirstOrDefault(e =>
                    e.Action.Label == actionLabel
                    && e.Type == ComandEffectType.ActionUpdateStatus
                    && e.Action.Status == ActionStatus.Error);
            Test.IsNotNull(action);
            _effects.Remove(action);
            return this;
        }

        public TestActionResults HasActionUpdateToStop(string actionLabel)
        {
            var action = _effects
                .FirstOrDefault(e =>
                    e.Action.Label == actionLabel
                    && e.Type == ComandEffectType.ActionUpdateStatus
                    && e.Action.Status == ActionStatus.ToStop);
            Test.IsNotNull(action);
            _effects.Remove(action);
            return this;
        }

        public TestActionResults HasActionUpdateStopped(string actionLabel)
        {
            var action = _effects
                .FirstOrDefault(e =>
                    e.Action.Label == actionLabel
                    && e.Type == ComandEffectType.ActionUpdateStatus
                    && e.Action.Status == ActionStatus.Stopped);
            Test.IsNotNull(action);
            _effects.Remove(action);
            return this;
        }

        public TestActionResults HasActionClearingCursor(string actionLabel)
        {
            var action = _effects
                .FirstOrDefault(e =>
                    e.Action.Label == actionLabel
                    && e.Type == ComandEffectType.ActionUpdateWithCursor
                    && e.Action.WithCursor == false);
            Test.IsNotNull(action);
            _effects.Remove(action);
            return this;
        }
        public TestActionResults HasActionSettingCursor(string actionLabel)
        {
            var action = _effects
                .FirstOrDefault(e =>
                    e.Action.Label == actionLabel
                    && e.Type == ComandEffectType.ActionUpdateWithCursor
                    && e.Action.WithCursor == true);
            Test.IsNotNull(action);
            _effects.Remove(action);
            return this;
        }

        public TestActionResults HasActionSettingBreakPoint(string actionLabel)
        {
            var action = _effects
                .FirstOrDefault(e =>
                    e.Action.Label == actionLabel
                    && e.Type == ComandEffectType.ActionUpdateBreakPoint
                    && e.Action.BreakPoint == true);
            Test.IsNotNull(action);
            _effects.Remove(action);
            return this;
        }

        public TestActionResults HasActionClearingBreakPoint(string actionLabel)
        {
            var action = _effects
                .FirstOrDefault(e =>
                    e.Action.Label == actionLabel
                    && e.Type == ComandEffectType.ActionUpdateBreakPoint
                    && e.Action.BreakPoint == false);
            Test.IsNotNull(action);
            _effects.Remove(action);
            return this;
        }
    }
}
