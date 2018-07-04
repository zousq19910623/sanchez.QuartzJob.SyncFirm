using Microsoft.Extensions.Configuration;
using SyncFirmToTbd.Quartz.Model;
using System;
using System.Collections.Specialized;

namespace SyncFirmToTbd.Quartz
{
    public class QuartzOption
    {
        public QuartzOption(IConfiguration config)
        {
            if (config == null)
            {
                throw new ArgumentNullException(nameof(config));
            }

            var section = config.GetSection("quartz");
            section.Bind(this);
        }

        public Scheduler Scheduler { get; set; }

        public ThreadPool ThreadPool { get; set; }

        public Plugin Plugin { get; set; }

        public NameValueCollection ToProperties()
        {
            var properties = new NameValueCollection
            {
                ["quartz.scheduler.instanceName"] = Scheduler?.InstanceName,
                ["quartz.threadPool.type"] = ThreadPool?.Type,
                ["quartz.threadPool.threadPriority"] = ThreadPool?.ThreadPriority,
                ["quartz.threadPool.threadCount"] = ThreadPool?.ThreadCount.ToString(),
                ["quartz.plugin.jobInitializer.type"] = Plugin?.JobInitializer?.Type,
                ["quartz.plugin.jobInitializer.fileNames"] = Plugin?.JobInitializer?.FileNames
            };

            return properties;
        }
    }
}
