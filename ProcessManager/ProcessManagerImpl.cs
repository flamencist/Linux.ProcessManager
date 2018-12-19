using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;

namespace Linux
{
    internal class ProcessManagerImpl : IProcessManager
    {
        public IEnumerable<int> EnumerateProcessIds()
        {
            foreach (var enumerateDirectory in Directory.EnumerateDirectories("/proc/"))
            {
                if (int.TryParse(Path.GetFileName(enumerateDirectory), NumberStyles.Integer,
                    CultureInfo.InvariantCulture, out var result))
                    yield return result;
            }
        }

        public ProcessInfo[] GetProcessInfos(IEnumerable<int> processIds, Func<ProcessInfo, bool> predicate)
        {
            var reusableReader = new ReusableTextReader();
            return processIds.Select(_ => CreateProcessInfo(_, reusableReader))
                .Where(_ => _ != null && predicate(_))
                .ToArray();
        }

        public List<string> GetCmdLine(int pid)
        {
            var specificDelimiterReader = new SpecificDelimiterTextReader();
            ProcFs.TryReadCommandLine(pid, out var cmdLine, specificDelimiterReader);
            return cmdLine;
        }

        public void Kill(int pid, int signal)
        {
            var result = Syscall.Kill(pid, signal);
            if (result != 0)
            {
                throw new Win32Exception(result, Syscall.GetLastError());
            }
        }

        private static ProcessInfo CreateProcessInfo(int pid, ReusableTextReader reusableReader = null)
        {
            if (reusableReader == null)
                reusableReader = new ReusableTextReader();
            if (!ProcFs.TryReadStatusFile(pid, out var result, reusableReader))
                return null;
            ProcFs.TryReadExeFile(pid, out var exe);
            ProcFs.TryReadCreationTime(pid, out var startTime);
            return CreateProcessInfo(result, exe, startTime);
        }

        private static ProcessInfo CreateProcessInfo(ProcFs.ParsedStatus procFsStatus, string exe, DateTime startTime)
        {
            var processInfo = new ProcessInfo
            {
                ProcessId = procFsStatus.Pid,
                ProcessName = procFsStatus.Name,
                State = procFsStatus.State,
                ParentProcessId = procFsStatus.Ppid,
                Ruid = procFsStatus.Ruid,
                Euid = procFsStatus.Euid,
                Egid = procFsStatus.Egid,
                Rgid = procFsStatus.Rgid,
                ExecutablePath = exe,
                StartTime = startTime
            };
            return processInfo;
        }
    }
}