using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;

namespace Linux
{
    internal static class ProcFs
    {
        private const string RootPath = "/proc/";
        private const string ExeFileName = "/exe";
        private const string StatusFileName = "/status";

        private static string GetStatusFilePathForProcess(int pid)
        {
            return RootPath + pid.ToString(CultureInfo.InvariantCulture) + StatusFileName;
        }

        internal static bool TryReadStatusFile(int pid, out ParsedStatus result, ReusableTextReader reusableReader)
        {
            var isParsed = TryParseStatusFile(GetStatusFilePathForProcess(pid), out result, reusableReader);
            Debug.Assert(!isParsed || result.Pid == pid,
                "Expected process ID from status file to match supplied pid");
            return isParsed;
        }

        private static bool TryParseStatusFile(string statusFilePath, out ParsedStatus result,
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
                result = default(ParsedStatus);
                return false;
            }

            var results = default(ParsedStatus);
            var dict = statusFileContents.Split(new []{Environment.NewLine}, StringSplitOptions.RemoveEmptyEntries)
                .Select(ToKeyValue)
                .ToDictionary(_ => _.Key, _ => _.Value.Trim(' ', '\t'));
            results.StatusContents = dict;
            results.Name = dict["Name"];
            results.State = dict["State"][0];
            results.Tgid = int.Parse(dict["Tgid"]);
            results.Ngid = int.Parse(dict["Ngid"]);
            results.Ppid = int.Parse(dict["PPid"]);
            results.Pid = int.Parse(dict["Pid"]);
            results.TracerPid = int.Parse(dict["TracerPid"]);
            var uids = dict["Uid"].Split('\t');
            results.Ruid = int.Parse(uids[0]);
            results.Euid = int.Parse(uids[1]);
            results.Suid = int.Parse(uids[2]);
            results.Fuid = int.Parse(uids[3]);
            var gids = dict["Gid"].Split('\t');
            results.Rgid = int.Parse(gids[0]);
            results.Egid = int.Parse(gids[1]);
            results.Sgid = int.Parse(gids[2]);
            results.Fgid = int.Parse(gids[3]);
            //results.FDSize = int.Parse(dict["FDSize"]);
            //results.Groups = dict["Groups"].Split(' ').Select(int.Parse).ToArray();
            //results.VmPeek = dict["VmPeek"];
            result = results;
            return true;
        }

        private static KeyValuePair<string, string> ToKeyValue(string source)
        {
            var chars = source.ToCharArray();
            var index = source.IndexOf(':');
            var key = new string(chars, 0, index);
            Debug.Assert(key != null);
            var valueStartIndex = index + 1;
            var value = new string(chars, valueStartIndex, source.Length - valueStartIndex);
            return new KeyValuePair<string, string>(key, value);
        }

        internal struct ParsedStatus
        {
            internal string Name;
            internal int Umask;
            internal char State;
            internal int Tgid;
            internal int Ngid;
            internal int Pid;
            internal int Ppid;
            internal int TracerPid;

            internal int Ruid;
            internal int Euid;
            internal int Suid;
            internal int Fuid;


            internal int Rgid;
            internal int Egid;
            internal int Sgid;
            internal int Fgid;

            //internal int FDSize;
            //internal int[] Groups;
            //internal int VmPeek; //KB
            //internal int VmSize; //KB

            internal Dictionary<string, string> StatusContents;
        }
    }
}