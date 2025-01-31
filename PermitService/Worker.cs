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
                Directory.SetCurrentDirectory(AppDomain.CurrentDomain.BaseDirectory);
                
                var fileProvider = new FileProvider();
                var csvFileManager = new CsvFileManager( logger, fileProvider, new DateTimeService(), ';');
                var inputRequestData = csvFileManager.ReadInputDataAsync(@"Data\input_data.csv");
                var savedRequestData = csvFileManager.ReadSavedDataAsync(@"Data\saved_data.csv");

                await Task.WhenAll(inputRequestData, savedRequestData);
                

                var requestData = savedRequestData.Result.Concat(inputRequestData.Result);

                await csvFileManager.SavePermitRequestData(@"Data\saved_data.csv", requestData.ToList());

                var monthNumbersToCheck = requestData.Select(x => x.GetDistinctMonthNumbers()).SelectMany(x => x).Distinct().ToList();

                if(monthNumbersToCheck.Count > 0)
                {
                    var webDriver = new FirefoxDriver();
                    webDriver.Navigate().GoToUrl(appSettings.Value.ScrapURL);
                    var webPageCrawler = new TeideWebPageClawler(webDriver);
                    var permitChecker = new PermitChecker(webPageCrawler);
                    var availableDays = permitChecker.GetAvailableDays(monthNumbersToCheck);
                    webDriver.Quit();
                }

                /*var smtpAdapter = new SmtpClientAdapter(new SmtpClientSettings());
                var emailNotification = new EmailNotification(smtpAdapter, appSettings.Value);
                await emailNotification.SendEmailAsync("some email", "some body", new System.Net.Mail.MailAddress("aaa"));*/
                
                
                await Task.Delay(appSettings.Value.RequestIntervalInSeconds * 1000, stoppingToken);
            }
            //hide firefox logs
            //hide firefox window
            //test invalid date time format 2025-06-5 in input data
            //provide the same entry startdate;enddate;email in input file as there is in saved file. Duplicated entries are not removed
            //cathc all exceptions
            //await opimalization
        }
    }
}
