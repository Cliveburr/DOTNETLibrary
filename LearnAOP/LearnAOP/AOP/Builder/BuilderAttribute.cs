using System;

namespace LearnAOP.AOP.Builder
{
    public class BuilderAttribute : Attribute
    {
        public virtual Type BuilderType { get; private set; }

        public BuilderAttribute()
        {
        }

        public BuilderAttribute(Type builderType)
        {
            BuilderType = builderType;
        }
    }
}