using System.Runtime;

namespace MauiBench.Helpers
{
    public class GCHelper
    {
        public static void CleanUp()
        {
            if (OperatingSystem.IsWindows() || OperatingSystem.IsLinux())
            {
                GCSettings.LargeObjectHeapCompactionMode = GCLargeObjectHeapCompactionMode.CompactOnce;
                GC.Collect(GC.MaxGeneration, GCCollectionMode.Forced, blocking: true);
                GC.WaitForPendingFinalizers();
                GC.Collect();
            }
        }
    }
}