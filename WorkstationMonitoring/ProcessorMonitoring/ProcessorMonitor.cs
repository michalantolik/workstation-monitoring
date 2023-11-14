using Client.Interfaces;
using System;

namespace ClientSubmodule.ProcessorMonitor
{
    public class ProcessorMonitor : IComponentMonitor
    {
        public ComponentType ComponentType => ComponentType.Processor;

        public string GetMonitorReport()
        {
            return $"This is a dummy info about {ComponentType} - {DateTime.Now}";
        }
    }
}
