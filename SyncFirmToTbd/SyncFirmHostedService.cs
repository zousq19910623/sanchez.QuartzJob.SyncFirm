using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Quartz;

namespace SyncFirmToTbd
{
    public class SyncFirmHostedService : IHostedService//, IDisposable
    {
        private readonly ILogger m_Logger;
        private readonly IScheduler m_Scheduler;


        public SyncFirmHostedService(ILogger<SyncFirmHostedService> logger, IScheduler scheduler)
        {
            m_Logger = logger;
            m_Scheduler = scheduler;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            m_Logger.LogInformation("Timed Background Service is starting.");

            await m_Scheduler.Start(cancellationToken);

            m_Logger.LogInformation("Timed Background Service is started.");

            //return Task.CompletedTask;
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            m_Logger.LogInformation("Timed Background Service is stopping.");

            await m_Scheduler.Shutdown(cancellationToken);

            m_Logger.LogInformation("Timed Background Service is stopped.");

            //return Task.CompletedTask;
        }

        //public void Dispose()
        //{
        //    throw new NotImplementedException();
        //}
    }
}
