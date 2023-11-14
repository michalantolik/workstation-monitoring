namespace Client.Interfaces
{
    /// <summary>
    /// Interface to be implemented by the particular workstation component monitor.
    /// </summary>
    /// <remarks>Disk, memory, processor etc.</remarks>
    public interface IComponentMonitor
    {
        /// <summary>
        /// Type of the monitored workstation component.
        /// </summary>
        public ComponentType ComponentType { get; }

        /// <summary>
        /// Returns JSON report about monitored workstation component.
        /// </summary>
        string GetMonitorReport();
    }
}
