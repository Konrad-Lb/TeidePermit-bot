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
using OpenQA.Selenium.BiDi.Modules.Network;

namespace PermitService
{
    public class Worker(ILog4NetAdapter logger, CsvFileManager csvFileManager, IOptions<AppSettings> appSettings) : BackgroundService
    {
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    logger.Info("Start checking permits.");
                    await CheckPermits();
                }
                catch (Exception ex)
                {
                    logger.Error($"Unexpected error while checking permits {ex.Message}");
                    LogInnerExceptionMessages(ex);
                }

                logger.Info("Checking permits finished.");
                await Task.Delay(appSettings.Value.RequestIntervalInSeconds * 1000, stoppingToken);
            }
        }

        private async Task CheckPermits()
        {
            var requestData = await ReadMergeAndSavePermitRequestData();
            var monthNumbersToCheck = requestData
                .Select(x => x.GetDistinctMonthNumbers())
                .SelectMany(x => x)
                .Distinct()
                .ToList();

            if (monthNumbersToCheck.Count > 0)
                await GetPermitAvailableDaysAndSendNotifications(monthNumbersToCheck, requestData);
            else
                logger.Warning("There are no months to check for all requests");
        }

        private async Task<IEnumerable<PermitRequestData>> ReadMergeAndSavePermitRequestData()
        {
            var inputRequestData = csvFileManager.ReadInputDataAsync(@"Data\input_data.csv");
            var savedRequestData = csvFileManager.ReadSavedDataAsync(@"Data\saved_data.csv");

            await Task.WhenAll(inputRequestData, savedRequestData);

            var requestData = savedRequestData.Result.Concat(inputRequestData.Result);
            await csvFileManager.SavePermitRequestData(@"Data\saved_data.csv", requestData.ToList());

            return requestData;
        }

        private async Task GetPermitAvailableDaysAndSendNotifications(List<Month> months, IEnumerable<PermitRequestData> requestData)
        {
            var availableDays = GetPermitAvailableDays(months);

            if (availableDays.Count > 0)
                await NotifyUsersAboutAvailablePermits(availableDays, requestData);
            else
                logger.Warning("There are no available days for all requests");
        }

        private IDictionary<Month, List<int>> GetPermitAvailableDays(List<Month> months)
        {
            var webDriver = new FirefoxDriver();
            webDriver.Navigate().GoToUrl(appSettings.Value.ScrapURL);
            var webPageCrawler = new TeideWebPageClawler(webDriver);
            var permitChecker = new PermitChecker(webPageCrawler);
            var permitAvailableDays = permitChecker.GetAvailableDays(months);
            webDriver.Quit();

            return permitAvailableDays;
        }

        private async Task NotifyUsersAboutAvailablePermits(IDictionary<Month, List<int>> availableDays, IEnumerable<PermitRequestData> requestData)
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

                LogInnerExceptionMessages(ex);
            }
        }

        private void LogInnerExceptionMessages(Exception ex)
        {
            var innerException = ex.InnerException;

            while (innerException != null)
            {
                logger.Error($"Inner exception message: {innerException.Message}");
                innerException = innerException.InnerException;
            }
        }

        //hide firefox logs
        //hide firefox window
        //test invalid date time format 2025-06-5 in input data
        //provide the same entry startdate;enddate;email in input file as there is in saved file. Duplicated entries are not removed
        //cathc all exceptions
        //await opimalization
    }
}
