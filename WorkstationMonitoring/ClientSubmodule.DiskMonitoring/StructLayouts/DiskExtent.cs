using System;
using System.Runtime.InteropServices;

namespace ClientSubmodule.DiskMonitor.StructLayouts
{
    [StructLayout(LayoutKind.Sequential)]
    public class DiskExtent
    {
        public UInt32 DiskNumber;
        public Int64 StartingOffset;
        public Int64 ExtentLength;
    }
}
