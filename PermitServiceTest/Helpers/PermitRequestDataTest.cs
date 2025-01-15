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
        [TestCase("2025-01-13;2025-03-15;user1@test.com", Description = "aaa")]
        [TestCase("2025-01-13;2025-03-15;user1@test.com;", Description = "vvv")]
        public void FromCSVString_PeroperCsvString_StringParsedCorrectly(string csvString)
        {
            var permitData = PermitRequestData.FromCsvString(csvString, ';');
            
            Assert.That(permitData.StartDate, Is.EqualTo(new DateTime(2025,1,13)));
            Assert.That(permitData.EndDate, Is.EqualTo(new DateTime(2025,3,15)));
            Assert.That(permitData.EmailAddress, Is.EqualTo("user1@test.com"));
        }

        [Test]
        public void FromCSVString_WrongDelimeterCharacter_ThrowInvalidOperationException()
        {
            var csvString = "2025-01-13;2025-03-15;user@test.com";
            var fieldDelimeter = '$';

            var exception = Assert.Throws<InvalidOperationException>(() => PermitRequestData.FromCsvString(csvString, fieldDelimeter));
            Assert.That(exception?.Message, Is.EqualTo($"Cannot create PermitRequestData object from CSV string '{csvString}'. CSV string does not have expected delimeter '{fieldDelimeter}'."));
        }

        [Test]
        public void FromCSVString_EmptyCsvString_ThrowInvalidOperationExeption()
        {
            var exception = Assert.Throws<InvalidOperationException>(() => PermitRequestData.FromCsvString(string.Empty, ';'));
            Assert.That(exception?.Message, Is.EqualTo("Cannot create PermitRequestData object from CSV string because CSV string is empty."));
        }

        [TestCase("date1", "date2", "date1", Description = "FromCSVString_WrongStartAndEndDateFormat_ThrowsInvalidOperationException")]
        [TestCase("2023-01-01", "date2", "date2", Description = "FromCSVString_WrongEndDateFormat_ThrowsInvalidOperationException")]
        public void FromCSVString_WrongDateValues_ThrowInvalidOprationException(string date1, string date2, string expectedStr)
        {
            var csvString = $"{date1};{date2};user@test.com";
            var fieldDelimeter = ';';

            var exception = Assert.Throws<InvalidOperationException>(() => PermitRequestData.FromCsvString(csvString, fieldDelimeter));
            Assert.That(exception?.Message, Is.EqualTo($"Cannot create PermitRequestData object from CSV string. CSV string contains not valid date time format {expectedStr}. Correct format is 'YYYY-MM-dd'"));
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

        //invalid number of fields
        //sasa;dsasd;vxcxvxc;
        //;;;
        //<date>;<date>;;
    }
}
