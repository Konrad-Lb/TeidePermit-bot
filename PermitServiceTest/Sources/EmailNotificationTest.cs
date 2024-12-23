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
        private Mock<ISmtpClientAdapter> _smtpClientMock = null!;
        private readonly Mock<IAppSettings> _appSettingsStub = new ();
        private readonly string _emailSubject = "emailSubject";
        private readonly string _emailBody = "emailBody";
        private readonly MailAddress _recipientEmailAddress = new ("to@test.com");

        [SetUp]
        public void TestSetUp()
        {
            _smtpClientMock = new Mock<ISmtpClientAdapter>();
            _appSettingsStub.Setup(x => x.SenderEmailAddress).Returns(new SenderEmailAddress("from@test.com", "sender_disp_name"));
        }

        [Test]
        public async Task SendEmailAsync_SendEmailWithSubjectAndBody_EmailSentToOnlyOneRecipient()
        {
            var emailNotification = new EmailNotification(_smtpClientMock.Object, _appSettingsStub.Object);
            await emailNotification.SendEmailAsync(_emailSubject, _emailBody, _recipientEmailAddress);

            _smtpClientMock.Verify(x =>
               x.SendAsync(It.Is<MailMessage>(mailMsg =>
                   mailMsg.To.Count() == 1
                   )),
               Times.Once());
        }

        [Test]
        public async Task SendEmailAsync_SendEmailWithSubjectAndBody_EmailSentWithProperMailMessage()
        {
            var emailNotification = new EmailNotification(_smtpClientMock.Object, _appSettingsStub.Object);
            await emailNotification.SendEmailAsync(_emailSubject, _emailBody, _recipientEmailAddress);

            _smtpClientMock.Verify(x => 
                x.SendAsync(It.Is<MailMessage>(mailMsg => 
                    mailMsg.From != null &&
                    _appSettingsStub.Object.SenderEmailAddress != null &&
                    mailMsg.From.Equals((MailAddress)_appSettingsStub.Object.SenderEmailAddress) &&
                    mailMsg.To[0].Equals(_recipientEmailAddress) &&
                    mailMsg.Subject == _emailSubject &&
                    mailMsg.Body == _emailBody
                    )),
                Times.Once());
        }

        [Test]
        public void SendEmailAsync_ExceptionWhileSendingEmail_ExceptionIsPropagated()
        {
            _smtpClientMock.Setup(x => x.SendAsync(It.IsAny<MailMessage>())).Throws<InvalidOperationException>();

            var emailNotification = new EmailNotification(_smtpClientMock.Object, _appSettingsStub.Object);
            Assert.ThrowsAsync<InvalidOperationException>(async () =>  await emailNotification.SendEmailAsync(_emailSubject, _emailBody, _recipientEmailAddress) );
        }
    }
}
