using System.Linq;

namespace Linux
{
    public static class ProcessManagerExtensions
    {
        public static int[] GetProcessIds(this IProcessManager processManager)
        {
            return processManager.EnumerateProcessIds().ToArray();
        }
        
        public static ProcessInfo GetProcessInfoById(this IProcessManager processManager, int pid)
        {
            return processManager.GetProcessInfos(new[] {pid}).FirstOrDefault();
        }
        
        public  static ProcessInfo[] GetProcessInfos(this IProcessManager processManager)
        {
            var processIds = processManager.GetProcessIds();
            return processManager.GetProcessInfos(processIds);
        }
    }
}