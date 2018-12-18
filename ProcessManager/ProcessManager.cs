using System.Collections.Generic;

namespace Linux
{
    public static class ProcessManager
    {
        public static IProcessManager Instance { get; } = new ProcessManagerImpl();
        
        public static IEnumerable<int> EnumerateProcessIds()
        {
            return Instance.EnumerateProcessIds();
        }

        public static int[] GetProcessIds()
        {
            return Instance.GetProcessIds();
        }

        public static ProcessInfo[] GetProcessInfos()
        {
            return Instance.GetProcessInfos();
        }
        
        public static ProcessInfo GetProcessInfoById(int pid)
        {
            return Instance.GetProcessInfoById(pid);
        }

        public static ProcessInfo[] GetProcessInfos(int[] processIds)
        {
            return Instance.GetProcessInfos(processIds);
        }

        public static List<string> GetCmdLine(int pid)
        {
            return Instance.GetCmdLine(pid);
        }
    }
}