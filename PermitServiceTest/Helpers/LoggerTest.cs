using log4net;
using log4net.Repository.Hierarchy;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PermitServiceTest.Helpers
{
    [TestFixture]
    public class LoggerTest
    {
        [Test]
        public void Info_LogMessage_AddedInfoPrefix()
        {
            var log4netMock = new Mock<ILog>();
            var logger = new PermitService.Helpers.Log4NetAdapter(log4netMock.Object);
            logger.Info("Info text");

            log4netMock.Verify(x => x.Info("[INFO] Info text"), Times.Once);
        }

        [Test]
        public void Info_LogMessage_AddedWarningPrefix()
        {
            var log4netMock = new Mock<ILog>();
            var logger = new PermitService.Helpers.Log4NetAdapter(log4netMock.Object);
            logger.Warning("Warning text");

            log4netMock.Verify(x => x.Warn("[WARNING] Warning text"), Times.Once);
        }

        [Test]
        public void Info_LogMessage_AddedErrorPrefix()
        {
            var log4netMock = new Mock<ILog>();
            var logger = new PermitService.Helpers.Log4NetAdapter(log4netMock.Object);
            logger.Error("Error text");

            log4netMock.Verify(x => x.Error("[ERROR] Error text"), Times.Once);
        }

        [Test]
        public void Info_LogMessage_AddedDebugPrefix()
        {
            var log4netMock = new Mock<ILog>();
            var logger = new PermitService.Helpers.Log4NetAdapter(log4netMock.Object);
            logger.Debug("Debug text");

            log4netMock.Verify(x => x.Debug("[DEBUG] Debug text"), Times.Once);
        }
    }
}
