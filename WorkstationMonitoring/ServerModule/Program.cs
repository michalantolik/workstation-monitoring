using ServerModule;

IHost host = Host.CreateDefaultBuilder(args)
    .UseWindowsService(options =>
    {
        options.ServiceName = "Workstation Monitoring Server Service";
    })
    .ConfigureServices(services =>
    {
        services.AddHostedService<ServerService>();
    })
    .Build();

await host.RunAsync();
