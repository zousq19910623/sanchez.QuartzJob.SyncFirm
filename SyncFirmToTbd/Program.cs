using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NLog.Extensions.Logging;
using NLog.Web;
using Quartz.Impl;
using Quartz.Spi;
using SyncFirmToTbd.Jobs;
using SyncFirmToTbd.Quartz;
using SyncFirmToTbd.Repository;
using SyncFirmToTbd.Services;
using SyncFirmToTbd.TBD.Services;
using System.IO;
using System.Threading.Tasks;

namespace SyncFirmToTbd
{
    class Program
    {
        static void Main(string[] args)
        {
            MainAsync().GetAwaiter().GetResult();
        }

        private static async Task MainAsync()
        {
            var host = new HostBuilder()
                .ConfigureHostConfiguration(configHost =>
                {
                    //配置根目录
                    configHost.SetBasePath(Directory.GetCurrentDirectory());

                    //读取host的配置json，和appsetting类似，暂不需要先注释掉，可根据需要开启
                    //configHost.AddJsonFile("hostsettings.json", true, true);

                    //读取环境变量，Asp.Net core默认的环境变量是以ASPNETCORE_作为前缀的，这里也采用此前缀以保持一致
                    configHost.AddEnvironmentVariables("ASPNETCORE_");

                    //可以在启动host的时候之前可传入参数，暂不需要先注释掉，可根据需要开启
                    //configHost.AddCommandLine(args);
                })
                .ConfigureAppConfiguration((hostContext, configApp) =>
                {
                    //读取应用的配置json
                    configApp.AddJsonFile("appsettings.json", true);

                    //读取应用特定环境下的配置json
                    configApp.AddJsonFile($"appsettings.{hostContext.HostingEnvironment.EnvironmentName}.json", true);

                    //读取环境变量
                    configApp.AddEnvironmentVariables();

                    //可以在启动host的时候之前可传入参数，暂不需要先注释掉，可根据需要开启
                    //configApp.AddCommandLine(args);
                })
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddLogging();
                    
                    services.AddSingleton<FirmService>();
                    services.AddSingleton<FirmRepository>();
                    services.AddSingleton<TbdFirmService>();

                    services.AddSingleton<IJobFactory, JobFactory>();
                    services.AddSingleton(provider =>
                    {
                        var option = new QuartzOption(hostContext.Configuration);
                        var sf = new StdSchedulerFactory(option.ToProperties());
                        var scheduler = sf.GetScheduler().Result;
                        scheduler.JobFactory = provider.GetService<IJobFactory>();
                        return scheduler;
                    });
                    services.AddHostedService<SyncFirmHostedService>();

                    services.AddSingleton<TestJob>();
                    services.AddSingleton<SyncFirmJob>();
                })
                .ConfigureLogging((hostContext, configLogging) =>
                {
                    //输出控制台日志
                    configLogging.AddConsole();

                    configLogging.AddNLog();
                    NLogBuilder.ConfigureNLog("nlog.config");

                    //开发环境输出Debug日志
                    if (hostContext.HostingEnvironment.EnvironmentName == EnvironmentName.Development)
                    {
                        configLogging.AddDebug();
                    }
                })
                .UseConsoleLifetime()//使用控制台生命周期  使用Ctrl+C退出
                .Build();
            
            await host.RunAsync();
        }
    }
}
