using Microsoft.Extensions.DependencyInjection;
using Runner.Kernel.Events.Command;
using Runner.Kernel.Events.Read;
using Runner.Kernel.Events.Write;
using Runner.Kernel.Mapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Runner.Kernel.Events
{
    public class EventProcess
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly EasyMapper _mapper;
        private readonly CancellationToken _cancellationToken;

        public EventProcess(IServiceProvider serviceProvider, EasyMapper mapper)
        {
            _serviceProvider = serviceProvider;
            _mapper = mapper;
            _cancellationToken = new CancellationToken();
        }

        public To MapTo<To>(object from)
        {
            return _mapper.MapTo<To>(from);
        }

        public Task Exec(IWrite request)
        {
            var handlerType = HandlerRegister.GetWriteHandler(request.GetType());
            var handler = _serviceProvider.GetRequiredService(handlerType);

            var handlerMethod = handlerType.GetMethod("Handler")!;
            var result = (Task)handlerMethod.Invoke(handler, [this, request, _cancellationToken])!;

            return result;
        }

        public Task<TResult> Exec<TResult>(IRead<TResult> request)
        {
            var handlerType = HandlerRegister.GetReadHandler(request.GetType());
            var handler = _serviceProvider.GetRequiredService(handlerType);

            var handlerMethod = handlerType.GetMethod("Handler")!;
            var result = (ValueTask<TResult>)handlerMethod.Invoke(handler, [this, request, _cancellationToken])!;

            return result.AsTask();
        }

        public  Task<TResult> Exec<TResult>(ICommandResult<TResult> request)
        {
        //    return ExecCast<ICommandResult<TResult>, TResult>(request);
        //}

        //private async Task<TResult> ExecCast<TRequest, TResult>(TRequest request) where TRequest : ICommandResult<TResult>
        //{
            var handlerType = HandlerRegister.GetCommandResultHandler(request.GetType());
            var handler = _serviceProvider.GetRequiredService(handlerType);
            //var handler = (ICommandResultHandler<TRequest, TResult>)handlerA;

            //todo: try/catch, get time

            var handlerMethod = handlerType.GetMethod("Handler")!;
            var result = (ValueTask<TResult>)handlerMethod.Invoke(handler, [this, request, _cancellationToken])!;

            //var result = await handler.Handler(this, request, _cancellationToken);
            
            return result.AsTask();
        }
    }
}
