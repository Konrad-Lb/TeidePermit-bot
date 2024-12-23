using PermitService.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace PermitService.Sources
{
    public class SmtpClientAdapter : ISmtpClientAdapter
    {
        private readonly SmtpClient _smtpClient;

        public SmtpClientAdapter(SmtpClientSettings smtpClientSettings)
        {
            _smtpClient = new SmtpClient
            {
                Host = smtpClientSettings.Host,
                Port = smtpClientSettings.Port,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = true,
                EnableSsl = smtpClientSettings.EnableSsl,
                Credentials = new NetworkCredential
                {
                    UserName = smtpClientSettings.UserName,
                    Password = smtpClientSettings.Password,
                }
            };
        }
        
        public SmtpDeliveryMethod DeliveryMethod
        {
            get => _smtpClient.DeliveryMethod;
            set => _smtpClient.DeliveryMethod = value;
        }

        public bool EnableSsl
        {
            get => _smtpClient.EnableSsl;
            set => _smtpClient.EnableSsl = value;
        }

        public bool UseDefaultCredentials
        {
            get => _smtpClient.UseDefaultCredentials;
            set => _smtpClient.UseDefaultCredentials = value;
        }

        public ICredentialsByHost? Credentials
        {
            get => _smtpClient.Credentials;
            set => _smtpClient.Credentials = value;
        }

        public void Send(MailMessage message)
        {
            _smtpClient.Send(message);
        }

        public async Task SendAsync(MailMessage message)
        {
            await _smtpClient.SendMailAsync(message);
        }
    }
}
