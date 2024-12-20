using System.Net;
using System.Net.Mail;

namespace PermitService.Sources
{
    public interface ISmtpClientAdapter
    {
        ICredentialsByHost? Credentials { get; set; }
        SmtpDeliveryMethod DeliveryMethod { get; set; }
        bool EnableSsl { get; set; }
        bool UseDefaultCredentials { get; set; }

        void Send(MailMessage message);
        Task SendAsync(MailMessage message);
    }
}