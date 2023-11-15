using Microsoft.AspNetCore.Builder;
using Serilog;
using ServerModule;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseWindowsService(options =>
{
    options.ServiceName = "Workstation Monitoring Server Service";
});

builder.Host.UseSerilog((hostingContext, loggerConfiguration) =>
{
    loggerConfiguration
        .WriteTo.Console()
        .WriteTo.File("serverLog.txt", rollingInterval: RollingInterval.Month);
});

builder.Services.AddSignalR();
builder.Services.AddHostedService<ServerService>();
builder.Services.AddLogging(loggingBuilder =>
{
    loggingBuilder.AddSerilog();
});

var app = builder.Build();

//--------------------------------------------------------------------
// Link "/reporthub" endpoint with SignalR "ReportHub"
//--------------------------------------------------------------------

app.MapHub<ReportHub>("/reporthub");

await app.RunAsync();
