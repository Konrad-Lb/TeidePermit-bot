using PermitService.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PermitService.Sources
{
    public class NotificationSender(IEmailSender emailSender)
    {
        public void SendMotification(IDictionary<Month, List<int>> availableDays, PermitRequestData requestData)
        {

        }
    }
}
