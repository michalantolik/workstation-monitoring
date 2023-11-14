using Client.Interfaces;

namespace ClientModule
{
    public class WorkstationInfoService
    {
        public IEnumerable<IComponentMonitor> Monitors { get; }

        public WorkstationInfoService(IEnumerable<IComponentMonitor> monitors)
        {
            Monitors = monitors;
        }

        //public IComponentMonitor this[ComponentType componentType]
        //{
        //    get => this._monitors.FirstOrDefault(monitor => monitor.ComponentType == componentType)!;
        //}

        //public string GetWorkstationJsonInfo(ComponentType componentType)
        //{
        //    var monitor = this[componentType];

        //    if (monitor == null)
        //    {
        //        return "";
        //    }

        //    var report = monitor.GetMonitorReport();

        //    return report;
        //}
    }
}
