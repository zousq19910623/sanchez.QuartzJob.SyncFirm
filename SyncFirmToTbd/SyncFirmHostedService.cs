using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace SyncFirmToTbd
{
    public class SyncFirmHostedService : IHostedService, IDisposable
    {
        private readonly ILogger m_Logger;

        public SyncFirmHostedService(ILogger<SyncFirmHostedService> logger)
        {
            m_Logger = logger;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            m_Logger.LogInformation("Timed Background Service is starting.");



            m_Logger.LogInformation("Timed Background Service is started.");

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            m_Logger.LogInformation("Timed Background Service is stopping.");



            m_Logger.LogInformation("Timed Background Service is stopped.");

            return Task.CompletedTask;
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}
