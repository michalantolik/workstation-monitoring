using Client.Interfaces;
using ClientSubmodule.DiskMonitor.Data;
using ClientSubmodule.DiskMonitor.StructLayouts;
using Microsoft.Win32.SafeHandles;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text.Json;

namespace ClientSubmodule.DiskMonitor
{
    public class DiskMonitor : IComponentMonitor
    {
        private readonly IWin32DiskApiWrapper diskApiWrapper;

        public DiskMonitor(IWin32DiskApiWrapper diskApiWrapper)
        {
            this.diskApiWrapper = diskApiWrapper;
        }

        public ComponentType ComponentType => ComponentType.Disk;

        public string GetMonitorReport()
        {
            var report = new VolumeDiskExtentsReportDto
            {
                MachineName = Environment.MachineName,
                Timestamp = DateTimeOffset.Now,
                VolumeDiskExtents = Array.Empty<VolumeDiskExtentDto>()
            };

            // Find all disk volume names
            var extentsList = new List<VolumeDiskExtentDto>();
            var succeeded = diskApiWrapper.TryFindVolumeNames(out var volumeNames);
            if (succeeded)
            {
                // NOTE: I don't reallly like this "TryDoSomething" and nested IF statements ...
                // I would do it differently next time ... ;))
                foreach (var singleVolumeName in volumeNames)
                {
                    // Find all disk volume paths
                    succeeded = diskApiWrapper.TryGetVolumePathsForVolumeName(singleVolumeName, out string[] volumePaths);

                    foreach (var singleVolumePath in volumePaths)
                    {
                        if (succeeded)
                        {
                            // Get safe file handle for a disk volume path
                            succeeded = diskApiWrapper.TryCreateVolumeSafeFileHandleFromVolumePath(
                                singleVolumePath,
                                out SafeFileHandle? volumeSfh,
                                out int? win32ErrorCode,
                                out string? win32ErrorMessage);

                            if (succeeded)
                            {
                                // Get disk extent for a safe file handle
                                succeeded = diskApiWrapper.TryGetVolumeDiskExtent
                                    (volumeSfh!,
                                    out IntPtr outBuffer,
                                    out DiskExtent? volumeExtent);

                                if (succeeded && volumeExtent != null)
                                {
                                    // Add disk extent to a disk report list
                                    var extents = new VolumeDiskExtentDto
                                    {
                                        VolumeDiskName = singleVolumeName,
                                        VolumeDiskPath = singleVolumePath,
                                        VolumeDiskNumber = (int)volumeExtent.DiskNumber,
                                        StartingOffset = volumeExtent.StartingOffset,
                                        ExtentLength = volumeExtent.ExtentLength,
                                    };
                                    extentsList.Add(extents);
                                }
                            }
                        }
                    }
                }

                report.VolumeDiskExtents = extentsList.ToArray();
            }

            string jsonReport = JsonSerializer.Serialize(report);

            return jsonReport;
        }
    }
}
