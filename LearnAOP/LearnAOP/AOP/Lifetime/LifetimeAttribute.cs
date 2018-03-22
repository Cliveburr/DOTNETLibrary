using System;

namespace LearnAOP.AOP.Lifetime
{
    public class LifetimeAttribute : Attribute
    {
        public virtual Type LifetimeType { get; private set; }

        public LifetimeAttribute()
        {
        }

        public LifetimeAttribute(Type lifetimeType)
        {
            LifetimeType = lifetimeType;
        }
    }
}