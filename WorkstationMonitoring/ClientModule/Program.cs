using Client.Interfaces;
using ClientModule;
using ClientSubmodule.DiskMonitor;
using ClientSubmodule.MemoryMonitor;
using ClientSubmodule.ProcessorMonitor;
using Serilog;

IHost host = Host.CreateDefaultBuilder(args)
    .UseWindowsService(options =>
    {
        options.ServiceName = "Workstation Monitoring Client Service";
    })
    .ConfigureServices(services =>
    {
        services.AddLogging(loggingBuilder =>
        {
            loggingBuilder.AddSerilog();
        });

        services.AddHostedService<ClientService>();

        services.AddSingleton<WorkstationInfoService>();
        services.AddSingleton<ReportHubService>();

        services.AddSingleton<IComponentMonitor, DiskMonitor>();
        services.AddSingleton<IWin32DiskApiWrapper, Win32DiskApiWrapper>();

        services.AddSingleton<IComponentMonitor, ProcessorMonitor>();

        services.AddSingleton<IComponentMonitor, MemoryMonitor>();
    })
    .UseSerilog((hostingContext, loggerConfiguration) =>
    {
        loggerConfiguration
            .WriteTo.Console()
            .WriteTo.File("clientLog.txt", rollingInterval: RollingInterval.Month);
    })
    .Build();

await host.RunAsync();
