using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace PermitService.Configuration
{
    public class AppSettings : IAppSettings
    {
        public int RequestIntervalInSeconds { get; set; }
        public SenderEmailAddress SenderEmailAddress { get; set; } = new SenderEmailAddress();
        public SmtpClientSettings SmtpClientSettings { get; set; } = new SmtpClientSettings();
        public string ScrapURL { get; set; } = string.Empty;
    }

}
