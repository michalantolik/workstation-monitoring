using Client.Interfaces;
using System;

namespace ClientSubmodule.DiskMonitor
{
    public class DiskMonitor : IComponentMonitor
    {
        public ComponentType ComponentType => ComponentType.Disk;

        public string GetMonitorReport()
        {
            return $"This is a dummy info about {ComponentType} - {DateTime.Now}";
        }
    }
}
