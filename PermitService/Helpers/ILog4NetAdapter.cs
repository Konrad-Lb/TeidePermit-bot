namespace PermitService.Helpers
{
    public interface ILog4NetAdapter
    {
        void Debug(string logMessage);
        void Error(string logMessage);
        void Info(string logMessage);
        void Warning(string logMessage);
    }
}