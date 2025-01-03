using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace PermitService.Sources
{
    public interface IEmailNotification
    {
        Task SendEmailAsync(string emailSubject, string emailBody, MailAddress recipientEmailAddress);
    }
}
