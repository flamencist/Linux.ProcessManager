using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;

namespace Linux
{
    internal static class ProcFs
    {
        private const string RootPath = "/proc/";
        private const string ExeFileName = "/exe";
        private const string CmdLineFileName = "/cmdline";
        private const string StatusFileName = "/status";
        private const string EnvironFileName = "/environ";
        private const string DefaultId = "-1";

        private static string GetRootPathForProcess(int pid)
        {
            return RootPath + pid.ToString(CultureInfo.InvariantCulture);
        }

        private static string GetStatusFilePathForProcess(int pid)
        {
            return RootPath + pid.ToString(CultureInfo.InvariantCulture) + StatusFileName;
        }

        private static string GetExeFilePathForProcess(int pid)
        {
            return RootPath + pid.ToString(CultureInfo.InvariantCulture) + ExeFileName;
        }

        private static string GetCmdLineFilePathForProcess(int pid)
        {
            return RootPath + pid.ToString(CultureInfo.InvariantCulture) + CmdLineFileName;
        }
        
        private static string GetEnvironFilePathForProcess(int pid)
        {
            return RootPath + pid.ToString(CultureInfo.InvariantCulture) + EnvironFileName;
        }

        internal static bool TryReadExeFile(int pid, out string exe)
        {
            var path = GetExeFilePathForProcess(pid);
            const int bufferSize = 2048;
            exe = null;

            var numArray = new byte[bufferSize + 1];
            var count = Syscall.ReadLink(path, numArray, bufferSize);
            if (count > 0)
            {
                numArray[count] = (byte) 0;
                exe = Encoding.UTF8.GetString(numArray, 0, count);
                return true;
            }

            Debug.WriteLine(Syscall.GetLastError());
            return false;
        }
        
        internal static bool TryReadEnvironFile(int pid, out IDictionary<string,string> environ, Func<KeyValuePair<string, string>,bool> predicate,
            SpecificDelimiterTextReader delimiterTextReader)
        {
            try
            {
                using (var source = new FileStream(GetEnvironFilePathForProcess(pid), FileMode.Open, FileAccess.Read,
                    FileShare.Read,
                    1, false))
                {
                    var query = delimiterTextReader.ReadLines(source)
                        .Select(_ => ToKeyValue(_, '='));
                    if (predicate != null)
                    {
                        query = query.Where(predicate);
                    }
                    environ = query.ToDictionary(_ => _.Key, _ => _.Value);

                    return true;
                }
            }
            catch (Exception e)
            {
                environ = new Dictionary<string,string>();
                Debug.WriteLine(e);
                return false;
            }
        }

        internal static bool TryReadCreationTime(int pid, out DateTime dateTime)
        {
            try
            {
                dateTime = File.GetCreationTimeUtc(GetRootPathForProcess(pid));
                return true;
            }
            catch (Exception e)
            {
                dateTime = default(DateTime);
                Debug.WriteLine(e);
                return false;
            }
        }

        internal static bool TryReadCommandLine(int pid, out List<string> cmdLine,
            SpecificDelimiterTextReader delimiterTextReader)
        {
            try
            {
                using (var source = new FileStream(GetCmdLineFilePathForProcess(pid), FileMode.Open, FileAccess.Read,
                    FileShare.Read,
                    1, false))
                {
                    cmdLine = delimiterTextReader.ReadLines(source).ToList();
                }

                return true;
            }
            catch (Exception e)
            {
                cmdLine = default(List<string>);
                Debug.WriteLine(e);
                return false;
            }
        }

        internal static bool TryReadStatusFile(int pid, out ParsedStatus result, ReusableTextReader reusableReader)
        {
            var isParsed = TryParseStatusFile(GetStatusFilePathForProcess(pid), out result, reusableReader);
            Debug.Assert(!isParsed || result.Pid == pid,
                "Expected process ID from status file to match supplied pid");
            return isParsed;
        }

        internal static bool TryParseStatusFile(string statusFilePath, out ParsedStatus result,
            ReusableTextReader reusableReader)
        {
            string statusFileContents;
            try
            {
                using (var source = new FileStream(statusFilePath, FileMode.Open, FileAccess.Read, FileShare.Read,
                    1, false))
                {
                    statusFileContents = reusableReader.ReadAllText(source);
                }
            }
            catch (IOException)
            {
                // Between the time that we get an ID and the time that we try to read the associated stat
                // file(s), the process could be gone.
                result = default;
                return false;
            }

            var results = default(ParsedStatus);
            var dict = statusFileContents.Split(new[] {Environment.NewLine}, StringSplitOptions.RemoveEmptyEntries)
                .Select(_=>ToKeyValue(_,':'))
                .ToDictionary(_ => _.Key, _ => _.Value.Trim(' ', '\t'));
            results.Name = dict.GetValueOrDefault("Name",string.Empty);
            results.State = dict.GetValueOrDefault("State"," ")[0];
            results.Ppid = int.Parse(dict.GetValueOrDefault("PPid", DefaultId));
            results.Pid = int.Parse(dict.GetValueOrDefault("Pid", DefaultId));
            
            var uids = dict.GetValueOrDefault("Uid",string.Empty).Split('\t');
            if (uids.Length >= 2)
            {
                results.Ruid = int.Parse(uids[0]);
                results.Euid = int.Parse(uids[1]);
            }
            else
            {
                results.Ruid = results.Euid  = -1;
            }

            var gids = dict.GetValueOrDefault("Gid",string.Empty).Split('\t');
            if (gids.Length >= 2)
            {
                results.Rgid = int.Parse(gids[0]);
                results.Egid = int.Parse(gids[1]);
            }
            else
            {
                results.Ruid = results.Euid = -1;
            }

            result = results;
            return true;
        }

        private static T GetValueOrDefault<T>(this IDictionary<string,T>dictionary, string key, T @default=default) => 
            dictionary.TryGetValue(key, out var result) ? result : @default;

        private static KeyValuePair<string, string> ToKeyValue(string source, char delimiter)
        {
            var chars = source.ToCharArray();
            var index = source.IndexOf(delimiter);
            var key = new string(chars, 0, index);
            Debug.Assert(key != null);
            var valueStartIndex = index + 1;
            var value = new string(chars, valueStartIndex, source.Length - valueStartIndex);
            return new KeyValuePair<string, string>(key, value);
        }

        internal struct ParsedStatus
        {
            internal string Name;
            internal char State;
            internal int Pid;
            internal int Ppid;

            internal int Ruid;
            internal int Euid;


            internal int Rgid;
            internal int Egid;
        }
    }
}