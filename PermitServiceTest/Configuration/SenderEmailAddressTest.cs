using NUnit.Framework;
using PermitService.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace PermitServiceTest.Configuration
{
    [TestFixture]
    public class SenderEmailAddressTest
    {
        [TestCase("test@test.com", "displayName", Description = "ConvertToMailAddress_DisplayNameIsNull_ObjectsAreEqual")]
        [TestCase("test@test.com", null, Description = "ConvertToMailAddress_DisplayNameIsNotNull_ObjectsAreEqual")]
        public void ConvertToMailAddress_ConstructorParametersAreTheSame_ObjectsAreEqual(string emailAddress, string? displayName)
        {
            var mailAddress = new MailAddress(emailAddress, displayName);
            var senderEmailAddress = new SenderEmailAddress(emailAddress, displayName);

            Assert.That(mailAddress.Equals((MailAddress)senderEmailAddress));
        }

        [Test]
        public void ConvertToMailAddress_WrongEmailAddressFormat_ThrowsFormatException()
        {
            var mailAddress = new MailAddress("test@test.com", "displayName");
            var senderEmailAddress = new SenderEmailAddress("aaa", "displayName");

            Assert.Throws<FormatException>(() => mailAddress.Equals((MailAddress)senderEmailAddress));
        }
    }
}
