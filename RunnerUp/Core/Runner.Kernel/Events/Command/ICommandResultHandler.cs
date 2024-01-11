using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Runner.Kernel.Events.Command
{
    public interface ICommandResultHandler<TRequest, TResult> where TRequest : ICommandResult<TResult>
    {
        Task<TResult> Handler(EventProcess process, TRequest request, CancellationToken cancellationToken);
    }
}
