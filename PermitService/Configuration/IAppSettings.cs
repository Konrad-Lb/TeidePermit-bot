namespace PermitService.Configuration
{
    public interface IAppSettings
    {
        int RequestIntervalInSeconds { get; set; }
        SenderEmailAddress SenderEmailAddress { get; set; }
        SmtpClientSettings SmtpClientSettings { get; set; }
    }
}