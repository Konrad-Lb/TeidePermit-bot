using log4net;
using log4net.Config;
using log4net.Repository;
using PermitService;
using PermitService.Configuration;
using PermitService.Helpers;
using PermitService.Sources;
using System.Reflection;


Directory.SetCurrentDirectory(AppDomain.CurrentDomain.BaseDirectory);
ILoggerRepository repository = LogManager.GetRepository(Assembly.GetExecutingAssembly());
XmlConfigurator.Configure(repository, new FileInfo("log4net.config"));
var logger = new Log4NetAdapter(log4net.LogManager.GetLogger("LOG"));
var fileProvider = new FileProvider();
var csvFileManager = new CsvFileManager(logger, fileProvider, new DateTimeService(), ';');

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddHostedService<Worker>();
builder.Services.Configure<AppSettings>(builder.Configuration.GetSection("AppSettings"));
builder.Services.AddSingleton<ILog4NetAdapter>(logger);
builder.Services.AddSingleton<CsvFileManager>(csvFileManager);

var host = builder.Build();
host.Run();
