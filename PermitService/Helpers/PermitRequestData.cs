using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PermitService.Helpers
{
    public class PermitRequestData
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string EmailAddress { get; set; } = string.Empty;

        public static PermitRequestData FromCsvString(string csvString, char fieldDelimater)
        {
            return new PermitRequestData
            {
                StartDate = ReadDateTime(ref csvString, fieldDelimater),
                EndDate = ReadDateTime(ref csvString, fieldDelimater),
                EmailAddress = csvString
            };
        }

        private static DateTime ReadDateTime(ref string line, char fieldDelimeter)
        {
            int lastDelimIndex = line.IndexOf(fieldDelimeter);
            var dateTime = DateTime.Parse(line[0..lastDelimIndex]);
            line = line.Remove(0, lastDelimIndex + 1);

            return dateTime;
        }
    }
}
