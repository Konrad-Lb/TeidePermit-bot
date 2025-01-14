using Moq;
using NUnit.Framework;
using PermitService.Helpers;
using PermitService.Sources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace PermitServiceTest.Sources
{
    [TestFixture]
    public class CsvFileManagerTest
    {
        [Test]
        public async Task ReadInputData_ThreeItemsInTheFile_ReturnedThreeEntries()
        {
            var fileProviderStub = new Mock<IFileProvider>();
            fileProviderStub.Setup(x => x.ReadLines(It.IsAny<string>())).ReturnsAsync([
                "2025-01-13;2025-01-13;user1@test.com", 
                "2025-02-01;2025-04-18;user2@test.com",
                "2025-01-02;2025-08-31;user3@test.com"]);
            
            var fileManager = new CsvFileManager(fileProviderStub.Object,';');
            var result =  await fileManager.ReadInputDataAsync("input.csv");

            Assert.That(result.Count, Is.EqualTo(3));
            Assert.That(result[0].StartDate, Is.EqualTo(new DateTime(2025,1,13)));
            Assert.That(result[0].EndDate, Is.EqualTo(new DateTime(2025, 1, 13)));
            Assert.That(result[0].EmailAddress, Is.EqualTo("user1@test.com"));
            Assert.That(result[1].StartDate, Is.EqualTo(new DateTime(2025, 2, 1)));
            Assert.That(result[1].EndDate, Is.EqualTo(new DateTime(2025, 4, 18)));
            Assert.That(result[1].EmailAddress, Is.EqualTo("user2@test.com"));
            Assert.That(result[2].StartDate, Is.EqualTo(new DateTime(2025, 1, 2)));
            Assert.That(result[2].EndDate, Is.EqualTo(new DateTime(2025, 8, 31)));
            Assert.That(result[2].EmailAddress, Is.EqualTo("user3@test.com"));
        }
    }

    //async optimalization
    //start date > endDate
    //date span is bigger than 12 months
    //rmoving input file
    //file not foundn
}
