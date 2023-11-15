using Client.Interfaces;
using ClientModule;
using ClientSubmodule.DiskMonitor;
using ClientSubmodule.MemoryMonitor;
using ClientSubmodule.ProcessorMonitor;

IHost host = Host.CreateDefaultBuilder(args)
    .UseWindowsService(options =>
    {
        options.ServiceName = "Workstation Monitoring Client Service";
    })
    .ConfigureServices(services =>
    {
        services.AddHostedService<ClientService>();

        services.AddSingleton<WorkstationInfoService>();
        services.AddSingleton<ReportHubService>();

        services.AddSingleton<IComponentMonitor, DiskMonitor>();
        services.AddSingleton<IWin32DiskApiWrapper, Win32DiskApiWrapper>();

        services.AddSingleton<IComponentMonitor, ProcessorMonitor>();

        services.AddSingleton<IComponentMonitor, MemoryMonitor>();
    })
    .Build();

await host.RunAsync();
