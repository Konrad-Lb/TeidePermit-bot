using Moq;
using NUnit.Framework;
using PermitService.Helpers;
using PermitService.Sources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace PermitServiceTest.Sources
{
    [TestFixture]
    public class NotificationSenderTest
    {
        [Test]
        public void SendNotification_NoAvailableDaysForRequestDataPeriod_NoEmailSent()
        {
            var emailSenderMock = new Mock<IEmailSender>();
            var requestData = new PermitRequestData { StartDate = new DateTime(2025, 1, 31), EndDate = new DateTime(2024, 2, 15), EmailAddress = "test@test.com"};
            var availableDays = new Dictionary<Month, List<int>> 
            {
                { Month.January, new List<int>{1, 26, 27, 28, 29, 30 } },
                { Month.February, new List<int>{16, 17, 18, 19, 20 } },
                { Month.March, new List<int> {1} }
            };

            new NotificationSender(emailSenderMock.Object).SendMotification(availableDays, requestData);

            emailSenderMock.Verify(x => x.SendEmailAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<MailAddress>()), Times.Never);

        }
    }

    //multilple days are available => send in one mail
    //one day availanle => send in one mail
    //no days available => send no email
}
