using Client.Interfaces;
using System;

namespace ClientSubmodule.MemoryMonitor
{
    public class MemoryMonitor : IComponentMonitor
    {
        public ComponentType ComponentType => ComponentType.Memory;

        public string GetMonitorReport()
        {
            return $"This is a dummy info about {ComponentType} - {DateTime.Now}";
        }
    }
}
