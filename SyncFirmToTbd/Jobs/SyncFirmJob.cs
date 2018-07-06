using Microsoft.Extensions.Logging;
using Quartz;
using System;
using System.Threading.Tasks;
using SyncFirmToTbd.Services;

namespace SyncFirmToTbd.Jobs
{
    public class SyncFirmJob : IJob
    {
        private ILogger m_Logger;
        private FirmService m_FirmService;

        public SyncFirmJob(ILogger<SyncFirmJob> logger, FirmService firmService)
        {
            m_Logger = logger;
            m_FirmService = firmService;
        }

        public Task Execute(IJobExecutionContext context)
        {
            m_Logger.LogInformation($"[{DateTime.Now:yyyy-MM-dd hh:mm:ss:ffffff}]正在同步公司..");

            var result = m_FirmService.PutNewEmployerToTbdAsync().Result;

            m_Logger.LogInformation($"[{DateTime.Now:yyyy-MM-dd hh:mm:ss:ffffff}]同步公司完成!!");

            return Task.CompletedTask;
        }
    }
}
