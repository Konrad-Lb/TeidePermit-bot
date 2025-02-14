using log4net.Config;
using log4net.Repository;
using log4net;
using Microsoft.Extensions.Options;
using PermitService.Configuration;
using PermitService.Helpers;
using PermitService.Sources;
using System.Reflection;
using System.Net.Http.Headers;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Firefox;

namespace PermitService
{
    public class Worker(ILog4NetAdapter logger, IOptions<AppSettings> appSettings) : BackgroundService
    {
        
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    logger.Info("Start checking permits.");
                    
                    Directory.SetCurrentDirectory(AppDomain.CurrentDomain.BaseDirectory);

                    var fileProvider = new FileProvider();
                    var csvFileManager = new CsvFileManager(logger, fileProvider, new DateTimeService(), ';');
                    var inputRequestData = csvFileManager.ReadInputDataAsync(@"Data\input_data.csv");
                    var savedRequestData = csvFileManager.ReadSavedDataAsync(@"Data\saved_data.csv");

                    await Task.WhenAll(inputRequestData, savedRequestData);

                    var requestData = savedRequestData.Result.Concat(inputRequestData.Result);

                    await csvFileManager.SavePermitRequestData(@"Data\saved_data.csv", requestData.ToList());

                    var monthNumbersToCheck = requestData.Select(x => x.GetDistinctMonthNumbers()).SelectMany(x => x).Distinct().ToList();

                    if (monthNumbersToCheck.Count > 0)
                    {
                        var webDriver = new FirefoxDriver();
                        webDriver.Navigate().GoToUrl(appSettings.Value.ScrapURL);
                        var webPageCrawler = new TeideWebPageClawler(webDriver);
                        var permitChecker = new PermitChecker(webPageCrawler);
                        var availableDays = permitChecker.GetAvailableDays(monthNumbersToCheck);
                        webDriver.Quit();

                        if (availableDays.Count > 0)
                        {
                            var sendTasks = new List<Task>();
                            foreach (var singleRequest in requestData)
                            {
                                var smtpAdapter = new SmtpClientAdapter(appSettings.Value.SmtpClientSettings);
                                var emailSender = new EmailSender(smtpAdapter, appSettings.Value);
                                sendTasks.Add(HandleSendEmailExceptionAsync(NotificationSender.SendNotification(emailSender, availableDays, singleRequest), singleRequest));
                            }
                            await Task.WhenAll(sendTasks);
                        }
                        else
                            logger.Warning("There are no available days for all requests");
                    }
                    else
                        logger.Warning("There are no months to check for all requests");
                }
                catch (Exception ex)
                {
                    logger.Error($"Unexpected error while checking permits {ex.Message}");

                    var innerException = ex.InnerException;

                    while (innerException != null)
                    {
                        logger.Error($"Inner exception message: {innerException.Message}");
                        innerException = innerException.InnerException;
                    }
                }

                logger.Info("Checking permits finished.");
                await Task.Delay(appSettings.Value.RequestIntervalInSeconds * 1000, stoppingToken);


                //hide firefox logs
                //hide firefox window
                //test invalid date time format 2025-06-5 in input data
                //provide the same entry startdate;enddate;email in input file as there is in saved file. Duplicated entries are not removed
                //cathc all exceptions
                //await opimalization
            }
        }

        private async Task HandleSendEmailExceptionAsync(Task task, PermitRequestData requestData)
        {
            try
            { 
                await task;
            }
            catch (Exception ex)
            {
                logger.Error($"Cannot send email notificiation for request permit: StartDate = {requestData.StartDate}, EndDate= {requestData.EndDate}, EmailAddress = {requestData.EmailAddress}");
                logger.Error($"Exception message: {ex.Message}");

                var innerException = ex.InnerException;

                while (innerException != null)
                {
                    logger.Error($"Inner exception message: {innerException.Message}");
                    innerException = innerException.InnerException;
                }
            }
        }
    }
}
