using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PermitService.Helpers
{
    public class Logger(ILog logger) : ILogger
    {
        public void Info(string logMessage)
        {
            logger.Info("[INFO] " + logMessage);
        }

        public void Error(string logMessage)
        {
            logger.Error("[ERROR] " + logMessage);
        }

        public void Warning(string logMessage)
        {
            logger.Warn("[WARNING] " + logMessage);
        }

        public void Debug(string logMessage)
        {
            logger.Debug("[DEBUG] " + logMessage);
        }
    }
}
