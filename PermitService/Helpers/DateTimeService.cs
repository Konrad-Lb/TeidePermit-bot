using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PermitService.Helpers
{
    public class DateTimeService : IDateTimeService
    {
        public DateTime CurrentDate { get => DateTime.Today; }
    }
}
