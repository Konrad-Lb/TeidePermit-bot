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
        [Test]
        public void FromCSVString_PeroperCsvString_StringParsedCorrectly()
        {
            var permitData = PermitRequestData.FromCsvString("2025-01-13;2025-03-15;user1@test.com", ';');
            
            Assert.That(permitData.StartDate, Is.EqualTo(new DateTime(2025,1,13)));
            Assert.That(permitData.EndDate, Is.EqualTo(new DateTime(2025,3,15)));
            Assert.That(permitData.EmailAddress, Is.EqualTo("user1@test.com"));
        }

        // deleimneter at the end of string
        //invalid string (csv entry in the file)
        //email is empty
        //input data has other delimeter than provided in constructor of CsvFileManager
    }
}
