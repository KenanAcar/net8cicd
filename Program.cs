using net8cicd.Configuration;
using net8cicd.Logging;
using net8cicd.Services;
using NLog;
using NLog.Web;

var logger = LogManager.Setup()
    .LoadConfigurationFromFile("nlog.config")
    .GetCurrentClassLogger();

logger.Info("App starting");
var builder = WebApplication.CreateBuilder(args);

builder.Logging.ClearProviders();
builder.Logging.SetMinimumLevel(Microsoft.Extensions.Logging.LogLevel.Trace);
builder.Host.ConfigureNLog();

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.ConfigureOpenTelemetry(builder.Configuration);
builder.Services.AddSingleton<TracingService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapGet("/test", async () =>
{
    logger.Info("Test endpoint called");
    using var client = new HttpClient();
    var exampleUri = builder.Configuration["ExampleUri"];
    var result = await client.GetStringAsync(exampleUri);
    logger.Info(result);
    return "Traced!";
});

app.MapGet("/sendLogs", (TracingService tracingService) =>
{
    tracingService.TraceSystemMetrics(); 
    logger.Info("Hello endpoint called");
    return "Hello World!";
});


app.Run();

