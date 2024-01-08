using Runner.Business.Tasks;
using Runner.Task.Hosting.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Runner.Task.Hosting.Tasks
{
    public class ProcessScriptTask : ITask
    {
        public ProcessScriptTask()
        {

        }

        public async Task<TaskExecuteResult> Execute(TaskExecuteRequest request)
        {
            // descompatar o script

            // checar as dll terminando com Script.dll

            // concluir gravando os node
            throw new NotImplementedException();
        }
    }
}
