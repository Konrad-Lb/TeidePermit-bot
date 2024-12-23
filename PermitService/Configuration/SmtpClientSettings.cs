using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PermitService.Configuration
{
    public class SmtpClientSettings
    {
        public string Host { get; set; } = String.Empty;
        public int Port { get; set; } = 0;
        public bool EnableSsl { get; set; } = true;
        public string UserName { get; set; } = String.Empty;
        public string Password { get; set; } = String.Empty;

    }
}
