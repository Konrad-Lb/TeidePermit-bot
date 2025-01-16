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
        private Mock<IFileProvider> _fileProviderMock = null!;

        [SetUp]
        public void TestSetUp()
        {
            _fileProviderMock = new Mock<IFileProvider>();
        }

        [TearDown]
        public void TestTearDown()
        {
            _fileProviderMock.Verify(x => x.FileExists(It.IsAny<string>()),Times.Once());
        }

        [Test]
        public async Task ReadInputData_ThreeItemsInTheFile_ReturnedThreeEntries()
        {
            _fileProviderMock = new Mock<IFileProvider>();
            _fileProviderMock.Setup(x => x.FileExists(It.IsAny<string>())).Returns(true);
            _fileProviderMock.Setup(x => x.ReadLines(It.IsAny<string>())).ReturnsAsync([
                "2025-01-13;2025-01-13;user1@test.com", 
                "2025-02-01;2025-04-18;user2@test.com",
                "2025-01-02;2025-08-31;user3@test.com"]);
            
            var fileManager = new CsvFileManager(_fileProviderMock.Object,';');
            var result =  (await fileManager.ReadInputDataAsync("input.csv")).ToList();

            _fileProviderMock.Verify(x => x.DeleteFile(It.IsAny<string>()), Times.Once);
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

        [Test]
        public async Task ReadInputData_InputFileNotExist_NoLinesRead()
        {
            _fileProviderMock.Setup(x => x.FileExists(It.IsAny<string>())).Returns(false);

            var fileManager = new CsvFileManager(_fileProviderMock.Object, ';');
            var result = await fileManager.ReadInputDataAsync("input.csv");

            _fileProviderMock.Verify(x => x.ReadLines(It.IsAny<string>()), Times.Never());
            _fileProviderMock.Verify(x => x.DeleteFile(It.IsAny<string>()), Times.Never);
            Assert.That(result.Count, Is.EqualTo(0));
        }

        [Test]
        public async Task ReadInPutData_InputFileCantBeDelete_NoLinesRead()
        {
            _fileProviderMock.Setup(x => x.FileExists(It.IsAny<string>())).Returns(true);
            _fileProviderMock.Setup(x => x.ReadLines(It.IsAny<string>())).ReturnsAsync(["2025-01-13;2025-01-13;user@test.com"]);
            _fileProviderMock.Setup(x => x.DeleteFile(It.IsAny<string>())).Throws<IOException>();

            var fileManager = new CsvFileManager(_fileProviderMock.Object, ';');
            var result = await fileManager.ReadInputDataAsync("input.csv");

            _fileProviderMock.Verify(x => x.ReadLines(It.IsAny<string>()), Times.Once);
            _fileProviderMock.Verify(x => x.DeleteFile(It.IsAny<string>()), Times.Once);
            Assert.That(result.Count, Is.EqualTo(0));
        }

        [Test]
        public async Task ReadInputData_InputFileCannotBeOpened_NoLinesRead()
        {
            _fileProviderMock.Setup(x => x.FileExists(It.IsAny<string>())).Returns(true);
            _fileProviderMock.Setup(x => x.ReadLines(It.IsAny<string>())).Throws<IOException>();

            var fileManager = new CsvFileManager(_fileProviderMock.Object, ';');
            var result = await fileManager.ReadInputDataAsync("input.csv");

            _fileProviderMock.Verify(x => x.ReadLines(It.IsAny<string>()), Times.Once);
            _fileProviderMock.Verify(x => x.DeleteFile(It.IsAny<string>()), Times.Never);
            Assert.That(result.Count, Is.EqualTo(0));
        }
    }

    //async optimalization
    //start date > endDate
    //date span is bigger than 12 months
    //file contained some crappy data
    //add log that file cannot be deleted
}
