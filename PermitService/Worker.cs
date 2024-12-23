using Microsoft.Extensions.Options;
using PermitService.Configuration;

namespace PermitService
{
    public class Worker(ILogger<Worker> logger, IOptions<AppSettings> appSettings) : BackgroundService
    {
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                if (logger.IsEnabled(LogLevel.Information))
                {
                    logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
                }
                await Task.Delay(appSettings.Value.RequestIntervalInSeconds * 1000, stoppingToken);
            }
        }
    }
}
