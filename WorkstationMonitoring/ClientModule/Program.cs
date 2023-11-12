using ClientModule;

IHost host = Host.CreateDefaultBuilder(args)
    .UseWindowsService(options =>
    {
        options.ServiceName = "Workstation Monitoring Client Service";
    })
    .ConfigureServices(services =>
    {
        services.AddHostedService<ClientService>();
        services.AddSingleton<ReportHubService>();
    })
    .Build();

await host.RunAsync();
