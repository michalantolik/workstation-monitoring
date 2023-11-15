using ClientSubmodule.DiskMonitor.StructLayouts;
using Microsoft.Win32.SafeHandles;
using System;

namespace ClientSubmodule.DiskMonitor
{
    public interface IWin32DiskApiWrapper
    {
        bool TryFindFirstVolumeName(out IntPtr hFindVolumePtr, out string volumeName);

        bool TryFindVolumeNames(out string[] volumeNames);

        bool TryGetVolumePathsForVolumeName(string volumeName, out string[] volumePaths, bool trimBackslashInPaths = false);

        bool TryCreateVolumeSafeFileHandleFromVolumePath(string volumePath, out SafeFileHandle? volumeSfh, out int? win32ErrorCode, out string? win32ErrorMessage);

        bool TryGetVolumeDiskExtent(SafeFileHandle diskFileHandle, out IntPtr outBuffer, out DiskExtent? diskExtent);
    }
}
