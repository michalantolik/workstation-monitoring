using Microsoft.AspNetCore.SignalR;

namespace ServerModule
{
    public class ReportHub : Hub
    {
        private readonly ILogger<ReportHub> _logger;

        public ReportHub(ILogger<ReportHub> logger)
        {
            _logger = logger;
        }

        public async Task SendReport(string report)
        {
            await Task.Run(() => _logger.LogInformation($"RECEIVED REPORT: {report}"));
        }
    }
}
