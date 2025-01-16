using log4net;
using log4net.Config;
using log4net.Repository;
using PermitService;
using PermitService.Configuration;
using PermitService.Helpers;
using System.Reflection;


ILoggerRepository repository = LogManager.GetRepository(Assembly.GetExecutingAssembly());
XmlConfigurator.Configure(repository, new FileInfo("log4net.config"));

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddHostedService<Worker>();
builder.Services.Configure<AppSettings>(builder.Configuration.GetSection("AppSettings"));
builder.Services.AddSingleton<ILog4NetAdapter>(new Log4NetAdapter(log4net.LogManager.GetLogger("LOG")));

var host = builder.Build();
host.Run();
