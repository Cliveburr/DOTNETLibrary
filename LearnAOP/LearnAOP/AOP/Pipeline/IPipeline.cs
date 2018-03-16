using System;
using System.Collections.Generic;
using System.Text;

namespace LearnAOP.AOP.Pipeline
{
    public interface IPipeline
    {
        void Execution(object data, NextPipelineDelegate next);
    }

    public delegate void NextPipelineDelegate(bool done);
}