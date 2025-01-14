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
        public void FromCSVString_WrongDelimeterCharacter_Throws()
        {
            var csvString = "2025-01-13;2025-03-15;user@test.com";
            var fieldDelimeter = '$';

            var exception = Assert.Throws<InvalidOperationException>(() => PermitRequestData.FromCsvString(csvString, fieldDelimeter));
            Assert.That(exception?.Message, Is.EqualTo($"Cannot create PermitRequestData object from CSV string '{csvString}'. CSV string does not have expected delimeter '{fieldDelimeter}'."));
        }

        //invalid string (csv entry in the file)
        //email is empty
        //input data has other delimeter than provided in constructor of CsvFileManager
        //invalid number of fields
        //sasa;dsasd;vxcxvxc;
    }
}
