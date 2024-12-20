using PermitService;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddHostedService<Worker>();
builder.Services.Configure<AppSettings>(builder.Configuration.GetSection("AppSettings"));

var host = builder.Build();
host.Run();
