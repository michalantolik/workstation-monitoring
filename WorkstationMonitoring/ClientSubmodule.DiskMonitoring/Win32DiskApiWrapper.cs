using System.Runtime.InteropServices;
using System.Text;
using System;
using System.Linq;
using Microsoft.Win32.SafeHandles;
using ClientSubmodule.DiskMonitor.StructLayouts;
using System.Collections.Generic;

namespace ClientSubmodule.DiskMonitor
{
    public class Win32DiskApiWrapper : IWin32DiskApiWrapper
    {
        const int MAX_PATH = 260;
        const uint FILE_SHARE_READ = 1;
        const uint OPEN_EXISTING = 3;
        const UInt32 IOCTL_VOLUME_GET_VOLUME_DISK_EXTENTS = 0x00560000;


        [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        static extern IntPtr FindFirstVolume(
            StringBuilder lpszVolumeName,
            uint cchBufferLength);

        [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        static extern bool FindNextVolume(
            IntPtr hFindVolume,
            StringBuilder lpszVolumeName,
            uint cchBufferLength);

        [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        static extern bool GetVolumePathNamesForVolumeName(
            string lpszVolumeName,
            StringBuilder lpszVolumePathNames,
            uint cchBufferLength,
            ref uint lpcchReturnLength);

        [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        static extern SafeFileHandle CreateFile(
                string lpFileName,
                uint dwDesiredAccess,
                uint dwShareMode,
                IntPtr lpSecurityAttributes,
                uint dwCreationDisposition,
                uint dwFlagsAndAttributes,
                IntPtr hTemplateFile);

        [DllImport("kernel32", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool DeviceIoControl(
            SafeFileHandle hDevice,
            UInt32 ioControlCode,
            IntPtr inBuffer,
            UInt32 inBufferSize,
            IntPtr outBuffer,
            UInt32 outBufferSize,
            out UInt32 bytesReturned,
            IntPtr overlapped);


        // Example of "volumeName": @"\\?\Volume{65e4c13e-c11f-4fd4-ac15-c9c64d781b81}\"
        public bool TryFindFirstVolumeName(out IntPtr hFindVolumePtr, out string volumeName)
        {
            try
            {
                var volumeNameSb = new StringBuilder(MAX_PATH);

                hFindVolumePtr = FindFirstVolume(volumeNameSb, (uint)volumeNameSb.Capacity);
                volumeName = volumeNameSb.ToString();

                return true;
            }
            catch
            {
                hFindVolumePtr = IntPtr.Zero;
                volumeName = String.Empty;

                return false;
            }
        }

        public bool TryFindVolumeNames(out string[] volumeNames)
        {
            var volumeNamesList = new List<string>();

            var succeeded = TryFindFirstVolumeName(out IntPtr hFindVolumePtr, out string firstVolumeName);

            if (!succeeded)
            {
                volumeNames = volumeNamesList.ToArray();
                return false;
            }
            volumeNamesList.Add(firstVolumeName);

            StringBuilder volumeNameSb = new StringBuilder();
            while (FindNextVolume(hFindVolumePtr, volumeNameSb, (uint)volumeNameSb.Capacity))
            {
                volumeNamesList.Add(volumeNameSb.ToString());
            }

            volumeNames = volumeNamesList.ToArray();
            return true;
        }

        // Example of "volumeName": @"\\?\Volume{65e4c13e-c11f-4fd4-ac15-c9c64d781b81}\"
        // Example of "volumePaths": { @"C:\" }
        public bool TryGetVolumePathsForVolumeName(string volumeName, out string[] volumePaths, bool trimBackslashInPaths = false)
        {
            StringBuilder volumePathsSb = new StringBuilder(MAX_PATH * 10);
            uint volumePathsSbCapacity = (uint)volumePathsSb.Capacity;
            uint returnLength = 0;

            var success = GetVolumePathNamesForVolumeName(volumeName, volumePathsSb, volumePathsSbCapacity, ref returnLength);

            if (!success)
            {
                volumePaths = Array.Empty<string>();
                return false;
            }

            volumePaths = volumePathsSb.ToString().Split('\0', StringSplitOptions.RemoveEmptyEntries);

            if (trimBackslashInPaths)
            {
                volumePaths = volumePaths.Select(s => s.TrimEnd('\\')).ToArray();
            }

            return true;
        }

        // Example of "volumePath": @"C:\"
        // Example of "hVolumeFilePtr": 0x0000000000000728 (1832)
        public bool TryCreateVolumeSafeFileHandleFromVolumePath(string volumePath, out SafeFileHandle? volumeSfh, out int? win32ErrorCode, out string? win32ErrorMessage)
        {
            try
            {
                string volumePathNoBackslash = volumePath.TrimEnd('\\');
                string lpFileName = $@"\\.\{volumePathNoBackslash}";

                volumeSfh = CreateFile(lpFileName, 0, FILE_SHARE_READ, IntPtr.Zero, OPEN_EXISTING, 0, IntPtr.Zero);

                //hVolumeFilePtr = CreateFile(lpFileName, 0, FILE_SHARE_READ, IntPtr.Zero, OPEN_EXISTING, 0, IntPtr.Zero);

                if (volumeSfh != null && !volumeSfh.IsInvalid)
                {
                    // succeeded

                    win32ErrorCode = null;
                    win32ErrorMessage = null;

                    return true;
                }

                // failed

                win32ErrorCode = Marshal.GetLastWin32Error();
                if (win32ErrorCode == 2)
                {
                    win32ErrorMessage = "Volume not found. Check if the volume name format is correct and if the volume exists.";
                }
                if (win32ErrorCode == 3)
                {
                    win32ErrorMessage = "Path not found. Check if the volume name format is correct and if the volume exists.";
                }
                else
                {
                    win32ErrorMessage = $"Unknown win 32 error. Check documentation for error code {win32ErrorCode} to find more info.";
                }

                return false;
            }
            catch
            {
                // failed

                volumeSfh = null;
                win32ErrorCode = null;
                win32ErrorMessage = null;

                return false;
            }
        }

        public bool TryGetVolumeDiskExtent(
            SafeFileHandle diskFileHandle,
            out IntPtr outBuffer,
            out DiskExtent? diskExtent)
        {
            VolumeDiskExtents volumeDiskExtents = new VolumeDiskExtents(); // get only first extent

            UInt32 outBufferSize = (UInt32)Marshal.SizeOf(volumeDiskExtents);
            outBuffer = Marshal.AllocHGlobal((int)outBufferSize);
            UInt32 bytesReturned = 0;

            if (!DeviceIoControl(diskFileHandle,
                                 IOCTL_VOLUME_GET_VOLUME_DISK_EXTENTS,
                                 IntPtr.Zero,
                                 0,
                                 outBuffer,
                                 outBufferSize,
                                 out bytesReturned,
                                 IntPtr.Zero))
            {
                diskExtent = null;
                return false;
            }

            // The call succeeded, so marshal the data back to a
            // form usable from managed code.

            Marshal.PtrToStructure(outBuffer, volumeDiskExtents);

            diskExtent = volumeDiskExtents.Extents;

            return true;
        }
    }
}
