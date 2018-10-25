using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;

namespace Linux
{
    public static class ProcessManager
    {
        public static IEnumerable<int> EnumerateProcessIds()
        {
            foreach (var enumerateDirectory in Directory.EnumerateDirectories("/proc/"))
            {
                if (int.TryParse(Path.GetFileName(enumerateDirectory), NumberStyles.Integer,
                    CultureInfo.InvariantCulture, out var result))
                    yield return result;
            }
        }

        public static int[] GetProcessIds()
        {
            return EnumerateProcessIds().ToArray();
        }

        public static ProcessInfo[] GetProcessInfos()
        {
            var processIds = GetProcessIds();
            return GetProcessInfos(processIds);
        }
        
        public static ProcessInfo GetProcessInfoByPid(int pid)
        {
            return GetProcessInfos(new[] {pid}).FirstOrDefault();
        }

        public static ProcessInfo[] GetProcessInfos(int[] processIds)
        {
            var reusableReader = new ReusableTextReader();
            var processInfoList = new List<ProcessInfo>(processIds.Length);
            foreach (var pid in processIds)
            {
                var processInfo = CreateProcessInfo(pid, reusableReader);
                if (processInfo != null)
                    processInfoList.Add(processInfo);
            }

            return processInfoList.ToArray();
        }

        public static List<string> GetCmdLine(int pid)
        {
            var specificDelimiterReader = new SpecificDelimiterTextReader();
            ProcFs.TryReadCommandLine(pid, out var cmdLine, specificDelimiterReader);
            return cmdLine;
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