using Microsoft.AspNetCore.Builder;
using ServerModule;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseWindowsService(options =>
{
    options.ServiceName = "Workstation Monitoring Server Service";
});

builder.Services.AddSignalR();
builder.Services.AddHostedService<ServerService>();

var app = builder.Build();

//--------------------------------------------------------------------
// Link "/reporthub" endpoint with SignalR "ReportHub"
//--------------------------------------------------------------------

app.MapHub<ReportHub>("/reporthub");

await app.RunAsync();
