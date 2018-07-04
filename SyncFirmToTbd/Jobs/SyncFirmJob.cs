using Microsoft.Extensions.Logging;
using Quartz;
using System;
using System.Threading.Tasks;

namespace SyncFirmToTbd.Jobs
{
    public class SyncFirmJob : IJob
    {
        private readonly ILogger m_Logger;

        public SyncFirmJob(ILogger<SyncFirmJob> logger)
        {
            m_Logger = logger;
        }

        public Task Execute(IJobExecutionContext context)
        {
            m_Logger.LogInformation($"[{DateTime.Now:yyyy-MM-dd hh:mm:ss:ffffff}]正在同步公司..");
            return Task.CompletedTask;
        }
    }
}
