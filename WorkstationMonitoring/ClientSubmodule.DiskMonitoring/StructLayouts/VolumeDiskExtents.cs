using System;
using System.Runtime.InteropServices;

namespace ClientSubmodule.DiskMonitor.StructLayouts
{
    [StructLayout(LayoutKind.Sequential)]
    public class VolumeDiskExtents
    {
        public UInt32 NumberOfDiskExtents;
        public DiskExtent Extents;
    }
}
