using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Quartz;
using System.Threading;
using System.Threading.Tasks;

namespace SyncFirmToTbd
{
    public class SyncFirmHostedService : IHostedService
    {
        private ILogger m_Logger;
        private IScheduler m_Scheduler;


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
    }
}
