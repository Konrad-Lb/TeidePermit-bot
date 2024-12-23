using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace PermitService.Configuration
{
    public class SenderEmailAddress
    {
        public SenderEmailAddress() { }
    
        public SenderEmailAddress(string emailAddress, string? displayName)
        {
            EmailAddress = emailAddress;
            DisplayName = displayName;
        }

        public static implicit operator MailAddress(SenderEmailAddress sender)
        {
            return new MailAddress(sender.EmailAddress, sender.DisplayName);
        }

        public string EmailAddress { get; set; } = String.Empty;
        public string? DisplayName { get; set; }
    }
}
