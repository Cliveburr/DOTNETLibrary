using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LearnAOP.AOP.Pipeline
{
    public class PipelineList
    {
        private IList<IPipeline> _list;

        private class RunContext
        {
            public object Data { get; set; }
            public int Index { get; set; }
            public bool IsDone { get; set; }
        }

        public PipelineList()
        {
            _list = new List<IPipeline>();
        }

        public PipelineList AddAtEnd(IPipeline pipe)
        {
            _list.Add(pipe);
            return this;
        }

        public void Run(object data)
        {
            var context = new RunContext
            {
                Data = data,
                Index = 0,
                IsDone = false
            };
            RunPipe(context);
        }

        private void RunPipe(RunContext context)
        {
            if (context.IsDone)
                return;

            NextPipelineDelegate nextFunc = delegate(bool done)
            {
                context.Index++;
                context.IsDone = done;
                RunPipe(context);
            };

            if (context.Index < _list.Count)
            {
                var pipe = _list[context.Index];
                pipe.Execution(context.Data, nextFunc);
            }
        }
    }
}