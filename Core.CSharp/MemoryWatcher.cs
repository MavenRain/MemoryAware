using System;
using Windows.System;

namespace Core.CSharp
{
    public static class MemoryWatcher
    {
        public static event EventHandler<object> ApplicationMemoryCriticalLimitReached;

        static MemoryWatcher()
        {
            MemoryManager.AppMemoryUsageIncreased += MemoryManager_AppMemoryUsageIncreased;
        }

        private static void MemoryManager_AppMemoryUsageIncreased(object sender, object e)
        {
            //Compute current app memory usage
            var usedMemoryMb = MemoryManager.AppMemoryUsage / 1000000;
            var totalMemoryUsageLimitMb = MemoryManager.AppMemoryUsageLimit / 1000000;
            var availableAppMemoryMb = totalMemoryUsageLimitMb - usedMemoryMb;
            //Test to see if usage is 95% or greater. If not, return.
            if (!(usedMemoryMb >= 0.95 * totalMemoryUsageLimitMb)) return;
            //If app memory usage is at least 95%, fire event
	        ApplicationMemoryCriticalLimitReached("Better clean up some resources!", availableAppMemoryMb);
        }
    }
}
