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
    public class EmailSender(ISmtpClientAdapter smtpClientAdapter, IAppSettings appSettings)  : IEmailSender
    {
        public async Task SendEmailAsync(string emailSubject, string emailBody, MailAddress recipientEmailAddress)
        {
            var mailMessage = CreateMailMessage(emailSubject, emailBody, recipientEmailAddress);   
            await smtpClientAdapter.SendAsync(mailMessage);
        }

        private MailMessage CreateMailMessage(string emailSubject, string emailBody, MailAddress recipientEmailAddress)
        {
            var mailMessage =  new MailMessage()
            {
                From = appSettings.SenderEmailAddress,
                Subject = emailSubject,
                Body = emailBody
            };

            mailMessage.To.Add(recipientEmailAddress);

            return mailMessage;
        }
    }
}
