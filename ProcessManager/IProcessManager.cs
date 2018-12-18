using System.Collections.Generic;

namespace Linux
{
    public interface IProcessManager
    {
        IEnumerable<int> EnumerateProcessIds();
        ProcessInfo[] GetProcessInfos(int[] processIds);
        List<string> GetCmdLine(int pid);
    }
}