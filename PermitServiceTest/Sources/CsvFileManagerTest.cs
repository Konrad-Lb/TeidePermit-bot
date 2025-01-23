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
        private Mock<ILog4NetAdapter> _logger = null!;
        private Mock<IDateTimeService> _dateTimeServiceStub = null!;

        [SetUp]
        public void TestSetUp()
        {
            _fileProviderMock = new Mock<IFileProvider>();
            _logger = new Mock<ILog4NetAdapter>();
            _dateTimeServiceStub = new Mock<IDateTimeService>();
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
            
            var fileManager = new CsvFileManager(_logger.Object, _fileProviderMock.Object, _dateTimeServiceStub.Object,';');
            var result =  (await fileManager.ReadInputDataAsync("input.csv")).ToList();

            _fileProviderMock.Verify(x => x.FileExists(It.IsAny<string>()), Times.Once);
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

            var fileManager = new CsvFileManager(_logger.Object, _fileProviderMock.Object, _dateTimeServiceStub.Object, ';');
            var result = await fileManager.ReadInputDataAsync("input.csv");

            _fileProviderMock.Verify(x => x.FileExists(It.IsAny<string>()), Times.Once());
            _fileProviderMock.Verify(x => x.ReadLines(It.IsAny<string>()), Times.Never());
            _fileProviderMock.Verify(x => x.DeleteFile(It.IsAny<string>()), Times.Never);
            Assert.That(result.Count, Is.EqualTo(0));
        }

        [Test]
        public async Task ReadInPutData_InputFileCantBeDeleted_NoLinesRead()
        {
            _fileProviderMock.Setup(x => x.FileExists(It.IsAny<string>())).Returns(true);
            _fileProviderMock.Setup(x => x.ReadLines(It.IsAny<string>())).ReturnsAsync(["2025-01-13;2025-01-13;user@test.com"]);
            _fileProviderMock.Setup(x => x.DeleteFile(It.IsAny<string>())).Throws<IOException>();

            var fileManager = new CsvFileManager(_logger.Object, _fileProviderMock.Object, _dateTimeServiceStub.Object, ';');
            var result = await fileManager.ReadInputDataAsync("input.csv");

            _fileProviderMock.Verify(x => x.FileExists(It.IsAny<string>()), Times.Once());
            _fileProviderMock.Verify(x => x.ReadLines(It.IsAny<string>()), Times.Once);
            _fileProviderMock.Verify(x => x.DeleteFile(It.IsAny<string>()), Times.Once);
            _logger.Verify(x => x.Warning("Input file cannot be opened or removed. Data in this file will be not consumed."));
            Assert.That(result.Count, Is.EqualTo(0));
        }

        [Test]
        public async Task ReadInputData_InputFileCannotBeOpened_LogExceptionMessageAndRetunsNoLines()
        {
            _fileProviderMock.Setup(x => x.FileExists(It.IsAny<string>())).Returns(true);
            _fileProviderMock.Setup(x => x.ReadLines(It.IsAny<string>())).Throws<IOException>();

            var fileManager = new CsvFileManager(_logger.Object, _fileProviderMock.Object, _dateTimeServiceStub.Object, ';');
            var result = await fileManager.ReadInputDataAsync("input.csv");

            _fileProviderMock.Verify(x => x.FileExists(It.IsAny<string>()), Times.Once());
            _fileProviderMock.Verify(x => x.ReadLines(It.IsAny<string>()), Times.Once);
            _fileProviderMock.Verify(x => x.DeleteFile(It.IsAny<string>()), Times.Never);
            _logger.Verify(x => x.Warning("Input file cannot be opened or removed. Data in this file will be not consumed."));
            Assert.That(result.Count, Is.EqualTo(0));
        }

        [Test]
        public async Task ReadInputData_ForUser2StartDateIsBiggerThanEndDate_WrongEntryIsNotReturned()
        {
            _fileProviderMock = new Mock<IFileProvider>();
            _fileProviderMock.Setup(x => x.FileExists(It.IsAny<string>())).Returns(true);
            _fileProviderMock.Setup(x => x.ReadLines(It.IsAny<string>())).ReturnsAsync([
                "2025-01-13;2025-01-13;user1@test.com",
                "2025-12-31;2025-04-18;user2@test.com",
                "2025-01-02;2025-08-31;user3@test.com"]);

            var fileManager = new CsvFileManager(_logger.Object, _fileProviderMock.Object, _dateTimeServiceStub.Object, ';');
            var result = (await fileManager.ReadInputDataAsync("input.csv")).ToList();

            _fileProviderMock.Verify(x => x.FileExists(It.IsAny<string>()), Times.Once());
            _fileProviderMock.Verify(x => x.DeleteFile(It.IsAny<string>()), Times.Once);
            _logger.Verify(x => x.Warning($"Entry in input file {{StartDate = 2025-12-31, EndDate = 2025-04-18 EmailAddress = user2@test.com}} has bigger StartDate then EndDate. It will be ignored"));
            Assert.That(result.Count, Is.EqualTo(2));
            Assert.That(result[0].StartDate, Is.EqualTo(new DateTime(2025, 1, 13)));
            Assert.That(result[0].EndDate, Is.EqualTo(new DateTime(2025, 1, 13)));
            Assert.That(result[0].EmailAddress, Is.EqualTo("user1@test.com"));
            Assert.That(result[1].StartDate, Is.EqualTo(new DateTime(2025, 1, 2)));
            Assert.That(result[1].EndDate, Is.EqualTo(new DateTime(2025, 8, 31)));
            Assert.That(result[1].EmailAddress, Is.EqualTo("user3@test.com"));
        }

        [Test]
        public async Task ReadInputData_DateSpanIsBiggerOrEquak12CalendarMonths_WrongEntriesAreNotReturned()
        {
            _fileProviderMock = new Mock<IFileProvider>();
            _fileProviderMock.Setup(x => x.FileExists(It.IsAny<string>())).Returns(true);
            _fileProviderMock.Setup(x => x.ReadLines(It.IsAny<string>())).ReturnsAsync([
                "2025-01-13;2025-12-31;user1@test.com",
                "2025-01-13;2026-01-01;user2@test.com",
                "2025-01-13;2026-02-01;user3@test.com",
                "2024-01-13;2024-12-31;user4@test.com",  //leap year
                "2024-01-13;2025-01-01;user5@test.com"   //leap year
            ]);

            var fileManager = new CsvFileManager(_logger.Object, _fileProviderMock.Object, _dateTimeServiceStub.Object, ';');
            var result = (await fileManager.ReadInputDataAsync("input.csv")).ToList();

            _fileProviderMock.Verify(x => x.FileExists(It.IsAny<string>()), Times.Once());
            _fileProviderMock.Verify(x => x.DeleteFile(It.IsAny<string>()), Times.Once);
            _logger.Verify(x => x.Warning($"Entry in input file {{StartDate = 2025-01-13, EndDate = 2026-01-01 EmailAddress = user2@test.com}} spans more than 12 calendar months. It will be ignored."));
            _logger.Verify(x => x.Warning($"Entry in input file {{StartDate = 2025-01-13, EndDate = 2026-02-01 EmailAddress = user3@test.com}} spans more than 12 calendar months. It will be ignored."));
            _logger.Verify(x => x.Warning($"Entry in input file {{StartDate = 2024-01-13, EndDate = 2025-01-01 EmailAddress = user5@test.com}} spans more than 12 calendar months. It will be ignored."));

            Assert.That(result.Count, Is.EqualTo(2));
            Assert.That(result[0].StartDate, Is.EqualTo(new DateTime(2025, 1, 13)));
            Assert.That(result[0].EndDate, Is.EqualTo(new DateTime(2025, 12, 31)));
            Assert.That(result[0].EmailAddress, Is.EqualTo("user1@test.com"));
            Assert.That(result[1].StartDate, Is.EqualTo(new DateTime(2024, 1, 13)));
            Assert.That(result[1].EndDate, Is.EqualTo(new DateTime(2024, 12, 31)));
            Assert.That(result[1].EmailAddress, Is.EqualTo("user4@test.com"));
        }

        [Test]
        public async Task ReadInputData_StartAndEndDatesAreInThePast_EntriesFromThePastAreNotReturned()
        {
            _dateTimeServiceStub.Setup(x => x.CurrentDate).Returns(new DateTime(2025, 1, 23));
            _fileProviderMock = new Mock<IFileProvider>();
            _fileProviderMock.Setup(x => x.FileExists(It.IsAny<string>())).Returns(true);
            _fileProviderMock.Setup(x => x.ReadLines(It.IsAny<string>())).ReturnsAsync([
                "2025-01-10;2025-01-20;user1@test.com",
            ]);

            var fileManager = new CsvFileManager(_logger.Object, _fileProviderMock.Object, _dateTimeServiceStub.Object, ';');
            var result = (await fileManager.ReadInputDataAsync("input.csv")).ToList();
            Assert.That(result.Count, Is.EqualTo(0));
        }

        [Test]
        public async Task ReadSavedData_SomeItemsInTheFile_SavedFileNotDeletedAfterRead()
        {
            _fileProviderMock = new Mock<IFileProvider>();
            _fileProviderMock.Setup(x => x.FileExists(It.IsAny<string>())).Returns(true);
            _fileProviderMock.Setup(x => x.ReadLines(It.IsAny<string>())).ReturnsAsync([
                "2025-01-13;2025-01-13;user1@test.com",
                "2025-02-01;2025-04-18;user2@test.com",
                "2025-01-02;2025-08-31;user3@test.com"]);

            var fileManager = new CsvFileManager(_logger.Object, _fileProviderMock.Object, _dateTimeServiceStub.Object, ';');
            var result = (await fileManager.ReadSavedDataAsync("saved.csv")).ToList();

            _fileProviderMock.Verify(x => x.FileExists(It.IsAny<string>()), Times.Once());
            _fileProviderMock.Verify(x => x.DeleteFile(It.IsAny<string>()), Times.Never);
        }

        [Test]
        public async Task ReadSavedData_InputFileCannotBeOpened_LogExceptionMessageAndRetunsNoLines()
        {
            _fileProviderMock.Setup(x => x.FileExists(It.IsAny<string>())).Returns(true);
            _fileProviderMock.Setup(x => x.ReadLines(It.IsAny<string>())).Throws<IOException>();

            var fileManager = new CsvFileManager(_logger.Object, _fileProviderMock.Object, _dateTimeServiceStub.Object, ';');
            var result = await fileManager.ReadSavedDataAsync("saved.csv");

            _fileProviderMock.Verify(x => x.FileExists(It.IsAny<string>()), Times.Once());
            _fileProviderMock.Verify(x => x.ReadLines(It.IsAny<string>()), Times.Once);
            _fileProviderMock.Verify(x => x.DeleteFile(It.IsAny<string>()), Times.Never);
            _logger.Verify(x => x.Warning("Saved file cannot be opended. Data saved in this file will be not read."));
            Assert.That(result.Count, Is.EqualTo(0));
        }

        [Test]
        public async Task SavePermitRequestData_EmptyPermitRequestDataList_FileNotCreated()
        {   
            var fileManager = new CsvFileManager(_logger.Object, _fileProviderMock.Object, _dateTimeServiceStub.Object, ';');
            await fileManager.SavePermitRequestData("output.csv",[]);

            _fileProviderMock.Verify(x => x.FileExists(It.IsAny<string>()), Times.Never);
            _fileProviderMock.Verify(x => x.WriteLines(It.IsAny<string>(), It.IsAny<List<string>>()), Times.Never);
        }

        [TestCase(true, Description = "SavePermitRequestData_EntryInPermitRequestDataListAndSaveFileExists_FileSaved")]
        [TestCase(false, Description = "SavePermitRequestData_EntryInPermitRequestDataListAndSaveFileNotExists_FileSaved")]
        public async Task SavePermitRequestData_EntryInPermitRequestDataList_FileSaved(bool fileExists)
        {
            _fileProviderMock.Setup(x => x.FileExists(It.IsAny<string>())).Returns(fileExists);

            var fileManager = new CsvFileManager(_logger.Object, _fileProviderMock.Object, _dateTimeServiceStub.Object, ';');
            await fileManager.SavePermitRequestData("output.csv", [new PermitRequestData { StartDate = new DateTime(2025,1,20), EndDate = new DateTime(2025,5,15), EmailAddress = "test@test.com" }]);

            _fileProviderMock.Verify(x => x.FileExists(It.IsAny<string>()), Times.Once);
            _fileProviderMock.Verify(x => x.DeleteFile(It.IsAny<string>()), fileExists ? Times.Once() : Times.Never());
            _fileProviderMock.Verify(x => x.WriteLines(It.IsAny<string>(), It.IsAny<IEnumerable<string>>()), Times.Once);
        }
    }
}
