using System;
using System.Collections.Generic;
using System.Text;

namespace LearnAOP.AOP.Builder
{
    public interface IBuilder
    {
        object Generate(ContainerType containerType);
    }
}