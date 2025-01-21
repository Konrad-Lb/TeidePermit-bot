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

        public bool IsStartDateBiggerThanEndDate()
        {
            return StartDate > EndDate;
        }

        public bool DatePeriodSpansOverTwelveMonths()
        {
            var startDate = new DateTime(StartDate.Year, StartDate.Month, 1);
            var endDate = new DateTime(EndDate.Year, EndDate.Month, 1);
            var monthSpan = endDate - startDate;

            return monthSpan >= new TimeSpan(365, 0, 0, 0);
        }

        public void AdjustStartDateToCurrentDate(IDateTimeService dateTimeService)
        {
            var startDatebeforeAdjust = StartDate;

            UpdateStartDateIfBiggerThan(dateTimeService.CurrentDate);
           
            if(IsStartDateBiggerThanEndDate())
            {
                StartDate = startDatebeforeAdjust;
                throw new InvalidOperationException("Cannot adjust PermitRequestData object as start and end dates will be in the past.");
            }
        }

        public void UpdateStartDateIfBiggerThan(DateTime dateTime)
        {
            if (StartDate < dateTime)
                StartDate = dateTime;
        }

        public static PermitRequestData FromCsvString(string csvString, char fieldDelimater)
        {
            ThrowExceptionIfCsvStringIsEmpty(csvString);
            RemoveLastCharIfStringEndsWithFieldDelimeter(ref csvString, fieldDelimater);

            return new PermitRequestData
            {
                StartDate = ReadDateTime(ref csvString, fieldDelimater),
                EndDate = ReadDateTime(ref csvString, fieldDelimater),
                EmailAddress = ReadEmailAddress(ref csvString, fieldDelimater)
            };
        }

        private static void ThrowExceptionIfCsvStringIsEmpty(string csvString)
        {
            ThrowExceptionIfStringIsEmpty(csvString, "Cannot create PermitRequestData object from CSV string because CSV string is empty.");
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
            catch (FormatException)
            {
                throw new InvalidOperationException($"Cannot create PermitRequestData object from CSV string. CSV string contains not valid date time format '{line[0..lastDelimIndex]}'. Correct format is 'YYYY-MM-dd'");
            }
        }

        private static string ReadEmailAddress(ref string line, char fieldDelimeter)
        {
            try
            {
                int lastDelimIndex = GetDelimeterIndexOrThrowExceptionIfNotFound(line, fieldDelimeter);
                return line[0..lastDelimIndex];
            }
            catch (InvalidOperationException)
            {
                ThrowEcxeptionIfEmailAddressEmpty(line);
                return line;
            }
        }

        private static void ThrowEcxeptionIfEmailAddressEmpty(string emaiAddress)
        {
            ThrowExceptionIfStringIsEmpty(emaiAddress, "Cannot create PermitRequestData object from CSV string. Email address field is empty.");
        }
        private static void ThrowExceptionIfStringIsEmpty(string text, string exceptionMessage)
        {
            if (String.IsNullOrEmpty(text))
                throw new InvalidOperationException(exceptionMessage);
        }

        private static int GetDelimeterIndexOrThrowExceptionIfNotFound(string csvString, char fieldDelimeter)
        {
            int delimIndex = csvString.IndexOf(fieldDelimeter);

            if (delimIndex < 0)
                throw new InvalidOperationException($"Cannot create PermitRequestData object from CSV string. CSV string does not have expected delimeter '{fieldDelimeter}' or number of CSV fields less than three.");
            return delimIndex;
        }

        public string ToCsvString(char fieldDelimeter)
        {
            return $"{StartDate:yyyy-MM-dd}{fieldDelimeter}{EndDate:yyyy-MM-dd}{fieldDelimeter}{EmailAddress}";
        }
    }
}
