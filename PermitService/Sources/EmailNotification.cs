using Microsoft.Extensions.Options;
using PermitService.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace PermitService.Sources
{
    public class EmailNotification : IEmailNotification
    {
        private readonly ISmtpClientAdapter _smtpClientAdapter;
        private readonly IAppSettings _appSettings;
        
        public EmailNotification(ISmtpClientAdapter smtpClientAdapter, IAppSettings appSettings)
        {
            _smtpClientAdapter = smtpClientAdapter;
            _appSettings = appSettings;
        }

        public async Task SendEmailAsync(string emailSubject, string emailBody)
        {
            var mailMessage = new MailMessage()
            {
                From = _appSettings.SenderEmailAddress,
                Subject = emailSubject,
                Body = emailBody
            };

            await _smtpClientAdapter.SendAsync(mailMessage);
        }
    }
}
