using System;
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
        
        public static ProcessInfo[] GetProcessInfos(this IProcessManager processManager, int[] processIds)
        {
            return processManager.GetProcessInfos(processIds,(info)=>true);
        }
        
        public static ProcessInfo[] GetProcessInfos(this IProcessManager processManager, Func<ProcessInfo,bool> predicate)
        {
            return processManager.GetProcessInfos(processManager.EnumerateProcessIds(), predicate);
        }

        public static bool TryKill(this IProcessManager processManager, int pid, int signal)
        {
            try
            {
                processManager.Kill(pid, signal);
                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }
    }
}