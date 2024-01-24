using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Runner.Agent.Version
{
    public static class WorkerEntry
    {
        [MethodImpl(MethodImplOptions.NoInlining)]
        public static bool Execute(IServiceProvider serviceProvider, CancellationToken stoppingToken)
        {
            using(var scope = serviceProvider.CreateScope())
            {
                var loggerFactory = scope.ServiceProvider.GetRequiredService<ILoggerFactory>();
                var logger = loggerFactory.CreateLogger<Worker>();
                var configuration = scope.ServiceProvider.GetRequiredService<IConfiguration>();

                var worker = new Worker(logger, configuration);
                return worker.ExecuteAsync(stoppingToken).Result;
            }



            //WeakReference workerRef;
            //var shouldReload = RunIsolated(serviceProvider, stoppingToken, out workerRef);
            ////var shouldReload = RunIsolated(serviceProvider, stoppingToken);

            //for (int i = 0; workerRef.IsAlive && (i < 10); i++)
            //{
            //    GC.Collect();
            //    GC.WaitForPendingFinalizers();
            //}

            //return shouldReload;
        }

        //[MethodImpl(MethodImplOptions.NoInlining)]
        //public static bool RunIsolated(IServiceProvider serviceProvider, CancellationToken stoppingToken, out WeakReference workerRef)
        //{
        //    using (var scope =  serviceProvider.CreateScope())
        //    {
        //        var loggerFactory = scope.ServiceProvider.GetRequiredService<ILoggerFactory>();
        //        //var logger = scope.ServiceProvider.GetRequiredService<ILogger<Worker>>();
        //        var logger = loggerFactory.CreateLogger<Worker>();
        //        var configuration = scope.ServiceProvider.GetRequiredService<IConfiguration>();

        //        var worker = new Worker(logger, configuration);
        //        workerRef = new WeakReference(worker, true);
        //        var shouldReload = worker.ExecuteAsync(stoppingToken).Result;

        //        return shouldReload;
        //    }
        //    //ILogger<Worker>? logger = null;
        //    //logger = null;
        //    //ILoggerProvider a;
        //    //a.CreateLogger


        //    //var shouldReload = false;

        //    //var i = 0;

        //    //while (i++ < 3)
        //    //{
        //    //    //_logger.LogInformation("Tick at: {time}", DateTimeOffset.Now);
        //    //    Console.WriteLine("tick");

        //    //    await Task.Delay(1000);
        //    //}

        //    //return false;
        //}
    }

    public class Worker
    {
        private readonly ILogger<Worker> _logger;
        private readonly IConfiguration _configuration;

        public Worker(ILogger<Worker> looger, IConfiguration configuration)
        {
            _logger = looger;
            _configuration = configuration;
        }

        public async Task<bool> ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Execute begin at: {time}", DateTimeOffset.Now);

            var i = 0;

            while (/*!stoppingToken.IsCancellationRequested*/ i++ < 3)
            {
                _logger.LogInformation("Tick at: {time}", DateTimeOffset.Now);

                await Task.Delay(1000);
            }

            _logger.LogInformation("Execute end at: {time}", DateTimeOffset.Now);

            return false;
        }
    }
}
