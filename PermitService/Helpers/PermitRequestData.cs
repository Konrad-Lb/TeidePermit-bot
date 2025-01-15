using System;
using System.Collections.Generic;
using System.Globalization;
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
            ThrowExceptionIfStringIsEmpty(csvString);
            RemoveLastCharIfStringEndsWithFieldDelimeter(ref csvString, fieldDelimater);
            
            return new PermitRequestData
            {
                StartDate = ReadDateTime(ref csvString, fieldDelimater),
                EndDate = ReadDateTime(ref csvString, fieldDelimater),
                EmailAddress = ReadEmailAddress(ref csvString, fieldDelimater)
            };
        }

        private static void ThrowExceptionIfStringIsEmpty(string csvString)
        {
            if (String.IsNullOrEmpty(csvString))
                throw new InvalidOperationException("Cannot create PermitRequestData object from CSV string because CSV string is empty.");
        }

        private static void RemoveLastCharIfStringEndsWithFieldDelimeter(ref string csvString, char fieldDelimater)
        {
            if (csvString.Last() == fieldDelimater)
                csvString = csvString.Remove(csvString.Length - 1, 1);
        }

        private static DateTime ReadDateTime(ref string line, char fieldDelimeter)
        {
            int lastDelimIndex = GetDelimeterIndexOrThrowExceptionIfNotFound(line, fieldDelimeter);
            try
            {
                var dateTime = DateTime.ParseExact(line[0..lastDelimIndex], "yyyy-MM-dd", CultureInfo.InvariantCulture);
                line = line.Remove(0, lastDelimIndex + 1);
                return dateTime;
            }
            catch(FormatException e)
            {
                throw new InvalidOperationException($"Cannot create PermitRequestData object from CSV string. CSV string contains not valid date time format {line[0..lastDelimIndex]}. Correct format is 'YYYY-MM-dd'");
            }
        }

        private static string ReadEmailAddress(ref string line, char fieldDelimeter)
        {
            try
            {
                int lastDelimIndex = GetDelimeterIndexOrThrowExceptionIfNotFound(line, fieldDelimeter);
                return line[0..lastDelimIndex];
            }
            catch(InvalidOperationException)
            {
                return line;
            }
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
