using NUnit.Framework;
using PermitService.Sources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PermitServiceTest.Sources
{
    [TestFixture]
    public class CsvFileManagerTest
    {
        [Test]
        public void ReadInputData_ThreeItemsInTheFile_ReturnedThreeEntries()
        {
            var fileManager = new CsvFileManager();
            var result =  fileManager.ReadInputData(@"Data\input.csv");

            Assert.That(result.Count, Is.EqualTo(3));
            Assert.That(result[0].StartDate, Is.EqualTo(new DateTime(2025,1,13)));
            Assert.That(result[0].EndDate, Is.EqualTo(new DateTime(2025, 1, 13)));
            Assert.That(result[0].EmailAddress, Is.EqualTo("user1@test"));
            Assert.That(result[1].StartDate, Is.EqualTo(new DateTime(2025, 2, 1)));
            Assert.That(result[1].EndDate, Is.EqualTo(new DateTime(2025, 4, 18)));
            Assert.That(result[1].EmailAddress, Is.EqualTo("user2@test"));
            Assert.That(result[2].StartDate, Is.EqualTo(new DateTime(2025, 1, 2)));
            Assert.That(result[2].EndDate, Is.EqualTo(new DateTime(2025, 8, 31)));
            Assert.That(result[2].EmailAddress, Is.EqualTo("user3@test"));
        }
    }

        //start date > endDate
        //invalid string (csv entry in the file)
        //email is empty
        //date span is bigger than 12 months
}
