using System;
using System.Collections.Generic;

namespace Linux
{
    public interface IProcessManager
    {
        IEnumerable<int> EnumerateProcessIds();
        ProcessInfo[] GetProcessInfos(IEnumerable<int> processIds, Func<ProcessInfo, bool> predicate);
        List<string> GetCmdLine(int pid);
    }
}