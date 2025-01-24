using Moq;
using Newtonsoft.Json.Linq;
using NUnit.Framework;
using PermitService.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PermitServiceTest.Helpers
{
    [TestFixture]
    public class PermitRequestDataTest
    {
        [TestCase("2025-01-13;2025-03-15;user1@test.com", Description = "FromCSVString_CsvStringEndedWithDelimeter_StringParsedCorrectly")]
        [TestCase("2025-01-13;2025-03-15;user1@test.com;", Description = "FromCSVString_CsvStringEndedWithoutDelimeter_StringParsedCorrectly")]
        public void FromCSVString_PeroperCsvString_StringParsedCorrectly(string csvString)
        {
            var permitData = PermitRequestData.FromCsvString(csvString, ';');

            Assert.That(permitData.StartDate, Is.EqualTo(new DateTime(2025, 1, 13)));
            Assert.That(permitData.EndDate, Is.EqualTo(new DateTime(2025, 3, 15)));
            Assert.That(permitData.EmailAddress, Is.EqualTo("user1@test.com"));
        }

        [TestCase("2025-01-13;2025-03-15;user@test.com", '$', Description = "FromCSVString_WrongDelimeterCharacter_ThrowInvalidOperationException")]
        [TestCase("2025-01-13", ';', Description = "FromCSVString_MissingEndDateAndEmailAddress_ThrowInvalidOperationException")]
        [TestCase("2025-01-13;2025-01-13", ';', Description = "FromCSVString_MissingEmailAddress_ThrowInvalidOperationException")]
        public void FromCSVString_WrongCsvString_ThrowInvalidOperationException(string csvString, char fieldDelimeter)
        {
            var exception = Assert.Throws<InvalidOperationException>(() => PermitRequestData.FromCsvString(csvString, fieldDelimeter));
            Assert.That(exception?.Message, Is.EqualTo($"Cannot create PermitRequestData object from CSV string. CSV string does not have expected delimeter '{fieldDelimeter}' or number of CSV fields less than three."));
        }

        [Test]
        public void FromCSVString_EmptyCsvString_ThrowInvalidOperationExeption()
        {
            var exception = Assert.Throws<InvalidOperationException>(() => PermitRequestData.FromCsvString(string.Empty, ';'));
            Assert.That(exception?.Message, Is.EqualTo("Cannot create PermitRequestData object from CSV string because CSV string is empty."));
        }

        [TestCase("date1", "date2", "user@test.com", "date1", Description = "FromCSVString_WrongStartAndEndDateFormat_ThrowsInvalidOperationException")]
        [TestCase("2023-01-01", "date2", "user@test.com", "date2", Description = "FromCSVString_WrongEndDateFormat_ThrowsInvalidOperationException")]
        [TestCase("", "", "", "", Description = "FromCSVString_EmpyFields_ThrowsInvalidOperationException")]
        public void FromCSVString_WrongDateValues_ThrowInvalidOprationException(string date1, string date2, string emailAddress, string expectedStr)
        {
            var csvString = $"{date1};{date2};{emailAddress}";
            var fieldDelimeter = ';';

            var exception = Assert.Throws<InvalidOperationException>(() => PermitRequestData.FromCsvString(csvString, fieldDelimeter));
            Assert.That(exception?.Message, Is.EqualTo($"Cannot create PermitRequestData object from CSV string. CSV string contains not valid date time format '{expectedStr}'. Correct format is 'YYYY-MM-dd'"));
        }

        [Test]
        public void FromCSVString_ExtraCsvFields_ExtraCsvFieldsAreIgnored()
        {
            var csvString = "2025-01-13;2025-03-15;user@test.com;additional_field";
            var fieldDelimeter = ';';

            var permitData = PermitRequestData.FromCsvString(csvString, fieldDelimeter);

            Assert.That(permitData.StartDate, Is.EqualTo(new DateTime(2025, 1, 13)));
            Assert.That(permitData.EndDate, Is.EqualTo(new DateTime(2025, 3, 15)));
            Assert.That(permitData.EmailAddress, Is.EqualTo("user@test.com"));
        }

        [Test]
        public void FromCSVString_EmptyEmailAddress_ThrowInvalidOperationException()
        {
            var csvString = "2025-01-13;2025-03-15;;";
            var fieldDelimeter = ';';

            var exception = Assert.Throws<InvalidOperationException>(() => PermitRequestData.FromCsvString(csvString, fieldDelimeter));
            Assert.That(exception?.Message, Is.EqualTo($"Cannot create PermitRequestData object from CSV string. Email address field is empty."));
        }

        [Test]
        public void IsStartDateBiggerThanEndDate_VariousPermitRequestDataObjects_MethodReturnsProperValues()
        {
            var obj1 = new PermitRequestData { StartDate = new DateTime(2025, 1, 13), EndDate = new DateTime(2025, 1, 14) };
            var obj2 = new PermitRequestData { StartDate = new DateTime(2025, 1, 13), EndDate = new DateTime(2025, 1, 13) };
            var obj3 = new PermitRequestData { StartDate = new DateTime(2025, 1, 13), EndDate = new DateTime(2025, 1, 12) };

            Assert.That(obj1.IsStartDateBiggerThanEndDate(), Is.False);
            Assert.That(obj2.IsStartDateBiggerThanEndDate(), Is.False);
            Assert.That(obj3.IsStartDateBiggerThanEndDate(), Is.True);
        }

        [Test]
        public void DatePeriodSpansOverTwelveMonths_VariousRequestDataObjects_MethodReturnsProperValues()
        {
            var obj1 = new PermitRequestData { StartDate = new DateTime(2025, 1, 13), EndDate = new DateTime(2025, 12, 31) };
            var obj2 = new PermitRequestData { StartDate = new DateTime(2025, 1, 13), EndDate = new DateTime(2026, 1, 1) };
            var obj3 = new PermitRequestData { StartDate = new DateTime(2025, 1, 13), EndDate = new DateTime(2026, 2, 1) };
            var obj4 = new PermitRequestData { StartDate = new DateTime(2024, 1, 13), EndDate = new DateTime(2024, 12, 31) }; //leap year
            var obj5 = new PermitRequestData { StartDate = new DateTime(2024, 1, 13), EndDate = new DateTime(2025, 1, 1) }; //leap year

            Assert.That(obj1.DatePeriodSpansOverTwelveMonths(), Is.False);
            Assert.That(obj2.DatePeriodSpansOverTwelveMonths(), Is.True);
            Assert.That(obj3.DatePeriodSpansOverTwelveMonths(), Is.True);
            Assert.That(obj4.DatePeriodSpansOverTwelveMonths(), Is.False);
            Assert.That(obj5.DatePeriodSpansOverTwelveMonths(), Is.True);
        }

        [Test]
        public void ToCsvString_PermitRequestData_ConvertToCsvString()
        {
            var obj1 = new PermitRequestData();
            var obj2 = new PermitRequestData { StartDate = new DateTime(2025, 1, 20), EndDate = new DateTime(2025, 5, 15), EmailAddress = "test@test.com" };

            Assert.That(obj1.ToCsvString(';'), Is.EqualTo("0001-01-01;0001-01-01;"));
            Assert.That(obj2.ToCsvString(';'), Is.EqualTo("2025-01-20;2025-05-15;test@test.com"));
            Assert.That(obj2.ToCsvString('@'), Is.EqualTo("2025-01-20@2025-05-15@test@test.com"));
        }

        [Test]
        public void AdjustDayPeriodToCurrentDay_StartDateIsBeforeCurrentData_StartDateAdjustedToCurrentDate()
        {
            var currentDateStrub = new Mock<IDateTimeService>();
            currentDateStrub.Setup(x => x.CurrentDate).Returns(new DateTime(2025, 1, 21));
            
            var permitRequestData = new PermitRequestData { StartDate = new DateTime(2025,1,1), EndDate = new DateTime(2025, 5, 15) };
            permitRequestData.AdjustStartDateToCurrentDate(currentDateStrub.Object);
            Assert.That(permitRequestData.StartDate, Is.EqualTo(currentDateStrub.Object.CurrentDate));
        }

        [TestCase(2025, 1, 21, Description = "AdjustDayPeriodToCurrentDay_StartDateIsEqualCurrentDate_DatePeriodNotChanged")]
        [TestCase(2025, 1, 31, Description = "AdjustDayPeriodToCurrentDay_StartDateIsAfterCurrentDate_DatePeriodNotChanged")]
        public void AdjustDayPeriodToCurrentDay_StartDateIsEqualOrAfterCurrentDate_DatePeriodNotChanged(int startYear, int startMonth, int startDay)
        {
            var currentDateStub = new Mock<IDateTimeService>();
            currentDateStub.Setup(x => x.CurrentDate).Returns(new DateTime(2025, 1, 21));

            var permitRequestData = new PermitRequestData { StartDate = new DateTime(startYear, startMonth, startDay), EndDate = new DateTime(2025, 5, 15) };
            permitRequestData.AdjustStartDateToCurrentDate(currentDateStub.Object);
            Assert.That(permitRequestData.StartDate, Is.EqualTo(new DateTime(startYear, startMonth, startDay)));
        }

        [Test]
        public void AdjustDayPeriodToCurrentDay_StartDateAndEndDateAreBeforeCurrentDate_DatePeriodNotChanged()
        {
            var currentDateStub = new Mock<IDateTimeService>();
            currentDateStub.Setup(x => x.CurrentDate).Returns(new DateTime(2025, 6, 6));

            var permitRequestData = new PermitRequestData { StartDate = new DateTime(2025, 1, 1), EndDate = new DateTime(2025, 5, 15) };

            var exception = Assert.Throws<InvalidOperationException>(() => permitRequestData.AdjustStartDateToCurrentDate(currentDateStub.Object));
            Assert.That(exception?.Message, Is.EqualTo("Cannot adjust PermitRequestData object as start and end dates will be in the past."));
            Assert.That(permitRequestData.StartDate, Is.EqualTo(new DateTime(2025, 1, 1)));
        }

        [Test]
        public void GetDistinctMonthNumbers_DiffrentDatePeriods_DistinctMonthNumbersReturned()
        {
            var permitRequestData1 = new PermitRequestData { StartDate = new DateTime(2025, 1, 24), EndDate = new DateTime(2025, 1, 28) };
            var permitRequestData2 = new PermitRequestData { StartDate = new DateTime(2025, 1, 24), EndDate = new DateTime(2025, 2, 1) };
            var permitRequestData3 = new PermitRequestData { StartDate = new DateTime(2025, 1, 24), EndDate = new DateTime(2026, 6, 2) };
            var permitRequestData4 = new PermitRequestData { StartDate = new DateTime(2025, 1, 31), EndDate = new DateTime(2025, 1, 1) };


            Assert.That(permitRequestData1.GetDistinctMonthNumbers(), Is.EqualTo([Month.January]));
            Assert.That(permitRequestData2.GetDistinctMonthNumbers(), Is.EqualTo([Month.January, Month.February]));
            Assert.That(permitRequestData3.GetDistinctMonthNumbers(), Is.EqualTo([Month.January, Month.February, Month.March, Month.April, Month.May, Month.June, Month.July, 
                                                                                 Month.August, Month.September, Month.October, Month.November, Month.December]));
            Assert.That(permitRequestData4.GetDistinctMonthNumbers(), Is.EqualTo(new List<int>()));
        }
    }
}
