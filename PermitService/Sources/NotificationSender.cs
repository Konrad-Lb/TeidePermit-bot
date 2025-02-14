using OpenQA.Selenium.BiDi.Modules.Network;
using PermitService.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace PermitService.Sources
{
    public static class NotificationSender
    {
        public static async Task SendNotification(IEmailSender emailSender, IDictionary<Month, List<int>> availableDays, PermitRequestData requestData)
        {
            var emailBody = new StringBuilder();
            var currentDate = requestData.StartDate;
            while(currentDate <= requestData.EndDate)
            {
                emailBody.Append(GenerateEmailBodyForDate(availableDays, currentDate));
                currentDate = currentDate.AddDays(1);
            }

           await SendEmailIfBodyNotEmpty(emailSender, emailBody.ToString(), requestData.EmailAddress);
        }

        private static string GenerateEmailBodyForDate(IDictionary<Month, List<int>> availableDays, DateTime date)
        {
            var month = (Month)date.Month;

            if (availableDays.TryGetValue(month, out var day) && day.Contains(date.Day))
                return $"New permits are avaliable for date {date.Day} of {month}\n";

            return string.Empty;
        }
        private static async Task SendEmailIfBodyNotEmpty(IEmailSender emailSender, string emailBody, string emailAddress)
        {
            if (!string.IsNullOrEmpty(emailBody) && !string.IsNullOrEmpty(emailAddress)) 
                await emailSender.SendEmailAsync("New Teide permits are available", emailBody, new MailAddress(emailAddress));
        }
    }
}
