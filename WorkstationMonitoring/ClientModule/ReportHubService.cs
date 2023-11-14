using Client.Interfaces;
using Microsoft.AspNetCore.SignalR.Client;

namespace ClientModule
{
    /// <summary>
    /// Service to connect to and to communicate with SignalR Hub.
    /// </summary>
    /// <remarks>It is NOT a service in terms of "Windows Services."</remarks> 
    /// <remarks>It is just a simple helper service to facilitate communication with SignalR Hub."</remarks> 
    public class ReportHubService
    {
        //--------------------------------------------------------------------
        // URL to SignalR Hub
        //--------------------------------------------------------------------

        private const string Url = "http://localhost:5000/reporthub";

        private readonly HubConnection _connection;
        private readonly ILogger<ReportHubService> _logger;

        public ReportHubService(ILogger<ReportHubService> logger)
        {
            _connection = new HubConnectionBuilder().WithUrl(Url).Build();
            _logger = logger;
        }

        public async Task ConnectAsync()
        {
            try
            {
                await _connection.StartAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "{Message}", ex.Message);
            }
        }

        public async void SendReport(string report)
        {
            await _connection.InvokeAsync("SendReport", report);

             _logger.LogInformation($"SENT REPORT: {report}");
        }
    }
}
