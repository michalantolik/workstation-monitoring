using Microsoft.Extensions.Configuration;

namespace ClientModule
{
    public class ClientService : BackgroundService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<ClientService> _logger;
        private readonly WorkstationInfoService _workstationInfoService;
        private readonly ReportHubService _reportHubService;

        private readonly int _sendingIntervalInMinutes;

        public ClientService(
            IConfiguration configuration,
            ILogger<ClientService> logger,
            WorkstationInfoService workstationInfoService,
            ReportHubService reportHubService)
        {
            _configuration = configuration;
            _logger = logger;
            _workstationInfoService = workstationInfoService;
            _reportHubService = reportHubService;

            //--------------------------------------------------------------------
            // Connect to SignalR Hub
            //--------------------------------------------------------------------

            Task.Run(() => _reportHubService.ConnectAsync()).Wait();

            //--------------------------------------------------------------------
            // Set reports sending interval (from asppsettings.json)
            //--------------------------------------------------------------------

            _sendingIntervalInMinutes = _configuration.GetValue<int>("Reports:ReportsSendingIntervalInMinutes");
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            try
            {
                while (!stoppingToken.IsCancellationRequested)
                {
                    //--------------------------------------------------------------------
                    // Send workstation monitor reports to SignalR Hub
                    //--------------------------------------------------------------------

                    foreach (var monitor in _workstationInfoService.Monitors)
                    {
                        var report = monitor.GetMonitorReport();

                        _reportHubService.SendReport(report);
                    }

                    await Task.Delay(TimeSpan.FromMinutes(_sendingIntervalInMinutes), stoppingToken);
                }
            }
            catch (OperationCanceledException)
            {
                // When the stopping token is canceled, for example, a call made from services.msc,
                // we shouldn't exit with a non-zero exit code. In other words, this is expected...
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "{Message}", ex.Message);

                // Terminates this process and returns an exit code to the operating system.
                // This is required to avoid the 'BackgroundServiceExceptionBehavior', which
                // performs one of two scenarios:
                // 1. When set to "Ignore": will do nothing at all, errors cause zombie services.
                // 2. When set to "StopHost": will cleanly stop the host, and log errors.
                //
                // In order for the Windows Service Management system to leverage configured
                // recovery options, we need to terminate the process with a non-zero exit code.
                Environment.Exit(1);
            }
        }
    }
}
