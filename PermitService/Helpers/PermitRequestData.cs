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
            RemoveLastCharIfStringEndsWithFieldDelimeter(ref csvString, fieldDelimater);
            
            return new PermitRequestData
            {
                StartDate = ReadDateTime(ref csvString, fieldDelimater),
                EndDate = ReadDateTime(ref csvString, fieldDelimater),
                EmailAddress = csvString
            };
        }

        private static void RemoveLastCharIfStringEndsWithFieldDelimeter(ref string csvString, char fieldDelimater)
        {
            if (csvString.Last() == fieldDelimater)
                csvString = csvString.Remove(csvString.Length - 1, 1);
        }

        private static DateTime ReadDateTime(ref string line, char fieldDelimeter)
        {
            int lastDelimIndex = GetDelimeterIndexOrThrowExceptionIfNotFound(line, fieldDelimeter);
            var dateTime = DateTime.Parse(line[0..lastDelimIndex]);
            line = line.Remove(0, lastDelimIndex + 1);

            return dateTime;
        }

        private static int GetDelimeterIndexOrThrowExceptionIfNotFound(string csvString, char fieldDelimeter)
        {
            int delimIndex = csvString.IndexOf(fieldDelimeter);

            if (delimIndex < 0)
                throw new InvalidOperationException($"Cannot create PermitRequestData object from CSV string '{csvString}'. CSV string does not have expected delimeter '{fieldDelimeter}'.");
            return delimIndex;
        }
    }
}
