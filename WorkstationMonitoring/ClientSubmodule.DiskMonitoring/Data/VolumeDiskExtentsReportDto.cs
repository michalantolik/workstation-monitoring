using System;

namespace ClientSubmodule.DiskMonitor.Data
{
    public class VolumeDiskExtentsReportDto
    {
        public string MachineName { get; set; }

        public DateTimeOffset Timestamp { get; set; }

        public VolumeDiskExtentDto[] VolumeDiskExtents { get; set; }

        public VolumeDiskExtentsReportDto()
        {
            MachineName = string.Empty;
            VolumeDiskExtents = Array.Empty<VolumeDiskExtentDto>();
        }
    }
}
