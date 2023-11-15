namespace ClientSubmodule.DiskMonitor.Data
{
    public class VolumeDiskExtentDto
    {
        public string? VolumeDiskName { get; set; }
        public string? VolumeDiskPath { get; set; }
        public int VolumeDiskNumber { get; set; }
        public long StartingOffset { get; set; }
        public long ExtentLength { get; set; }
    }
}
