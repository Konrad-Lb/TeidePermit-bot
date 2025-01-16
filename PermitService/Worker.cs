using log4net.Config;
using log4net.Repository;
using log4net;
using Microsoft.Extensions.Options;
using PermitService.Configuration;
using PermitService.Helpers;
using PermitService.Sources;
using System.Reflection;

namespace PermitService
{
    public class Worker(ILog log4netLogger, IOptions<AppSettings> appSettings) : BackgroundService
    {
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
         
            
            while (!stoppingToken.IsCancellationRequested)
            {
                await Task.Delay(appSettings.Value.RequestIntervalInSeconds * 1000, stoppingToken);
            }
        }
    }
}
