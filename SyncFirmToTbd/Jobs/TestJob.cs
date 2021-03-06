﻿using Microsoft.Extensions.Logging;
using Quartz;
using System;
using System.Threading.Tasks;

namespace SyncFirmToTbd.Jobs
{
    public class TestJob : IJob
    {
        private readonly ILogger m_Logger;

        public TestJob(ILogger<TestJob> logger)
        {
            m_Logger = logger;
        }

        public Task Execute(IJobExecutionContext context)
        {
            m_Logger.LogInformation($"[{DateTime.Now:yyyy-MM-dd hh:mm:ss:ffffff}]任务执行！");
            return Task.CompletedTask;
        }
    }
}