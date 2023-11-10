using ClientModule;

IHost host = Host.CreateDefaultBuilder(args)
    .UseWindowsService(options =>
    {
        options.ServiceName = "Workstation Monitoring Client Service";
    })
    .ConfigureServices(services =>
    {
        services.AddHostedService<ClientService>();
    })
    .Build();

await host.RunAsync();
