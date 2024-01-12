using Microsoft.Extensions.DependencyInjection;
using Runner.Kernel.Events;
using Runner.Kernel.Events.Command;
using Runner.Kernel.Mapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Runner.Kernel.Services
{
    public class KernelService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly EasyMapper _mapper;

        public KernelService(IServiceProvider serviceProvider, EasyMapper mapper)
        {
            _serviceProvider = serviceProvider;
            _mapper = mapper;
        }

        public Task<TResult> Exec<TResult>(ICommandResult<TResult> commandResult)
        {
            var eventProcess = new EventProcess(_serviceProvider, _mapper);

            var result = eventProcess.Exec(commandResult);

            //TODO: salvar resultado?

            return result;
        }

        public async Task Exec(ICommand command)
        {
            var eventProcess = new EventProcess(_serviceProvider, _mapper);

            await eventProcess.Exec(command);

            //TODO: salvar resultado?
        }

        //public Task<TResult> WatchOn<TResult>(ICommandResult<TResult> commandResult)
        //{

        //}
    }
}
