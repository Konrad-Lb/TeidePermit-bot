using Microsoft.Extensions.Options;
using Moq;
using NUnit.Framework;
using PermitService.Configuration;
using PermitService.Sources;
using System.Net.Mail;

namespace PermitServiceTest.Sources
{
    [TestFixture]
    public class EmailNotificationTest
    {
        [Test]
        public async Task SendEmailAsync_SendEmailWithSubjectAndBody_EmailSentWithProperMailMessage()
        {
            var smtpClientMock = new Mock<ISmtpClientAdapter>();
            var appSettingsStub = new Mock<AppSettings>();
            var emailSubject = "emailSubject";
            var emailBody = "emailBody";
            
            var emailNotification = new EmailNotification(smtpClientMock.Object, appSettingsStub.Object);
            await emailNotification.SendEmailAsync(emailSubject, emailBody);

            smtpClientMock.Verify(x => 
                x.SendAsync(It.Is<MailMessage>(mailMsg => 
                    mailMsg.Subject == emailSubject &&
                    mailMsg.Body == emailBody
                    )),
                Times.Once());
        }
    }
}
