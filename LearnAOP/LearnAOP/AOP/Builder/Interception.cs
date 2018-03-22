using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace LearnAOP.AOP.Builder
{
    public interface IInterception
    {
        bool HasPreExecute { get; }
        void PreExecute(InterceptionRunContext context);
        bool HasErrorExecute { get; }
        void PosExecute(InterceptionRunContext context);
        bool HasPosExecute { get; }
        void ErrorExecute(InterceptionRunContext context);
    }

    public abstract class InterceptionAttribute : Attribute, IInterception
    {
        public virtual bool HasPreExecute => false;
        public virtual bool HasErrorExecute => false;
        public virtual bool HasPosExecute => false;

        public virtual void ErrorExecute(InterceptionRunContext context)
        {
        }

        public virtual void PosExecute(InterceptionRunContext context)
        {
        }

        public virtual void PreExecute(InterceptionRunContext context)
        {
        }
    }

    public abstract class InterceptionQuery : IInterception
    {
        public abstract bool IsApply(MethodInfo method);
        public virtual bool HasPreExecute => false;
        public virtual bool HasErrorExecute => false;
        public virtual bool HasPosExecute => false;

        public virtual void ErrorExecute(InterceptionRunContext context)
        {
        }

        public virtual void PosExecute(InterceptionRunContext context)
        {
        }

        public virtual void PreExecute(InterceptionRunContext context)
        {
        }
    }

    public class InterceptionRunContext
    {
        public object Object { get; set; }
        public string MethodName { get; set; }
        public object[] Parameters { get; set; }
        public object Return { get; set; }
        public Exception Exception { get; set; }
        public bool RaiseException { get; set; }
    }
}