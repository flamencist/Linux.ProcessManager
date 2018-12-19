using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace Linux
{
    internal static class Syscall
    {
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
        internal struct Passwd
        {
            [MarshalAs(UnmanagedType.LPStr)] internal string pw_name;
            [MarshalAs(UnmanagedType.LPStr)] internal string pw_passwd;
            internal uint pw_uid;
            internal uint pw_gid;
            [MarshalAs(UnmanagedType.LPStr)] internal string pw_gecos;
            [MarshalAs(UnmanagedType.LPStr)] internal string pw_dir;
            [MarshalAs(UnmanagedType.LPStr)] internal string pw_shell;
        }


        [DllImport("libc", EntryPoint = "readlink", SetLastError = true)]
        internal static extern int ReadLink(string path, byte[] buf, int bufsiz);

        [DllImport("libc", EntryPoint = "geteuid", SetLastError = true)]
        internal static extern int GetEffectiveUserId();

        [DllImport("libc", EntryPoint = "getpwuid", SetLastError = true)]
        private static extern IntPtr sys_getpwuid(int uid);
        
        [DllImport("libc", EntryPoint = "getpwnam", SetLastError = true)]
        internal static extern IntPtr getpwnam(string name);

        internal static Passwd GetPasswdByUserId(int uid)
        {
            try
            {
                var ptr = sys_getpwuid(uid);
                return ptr == IntPtr.Zero ? new Passwd() : Marshal.PtrToStructure<Passwd>(ptr);
            }
            catch (Exception e)
            {
                Trace.WriteLine(e);
                return new Passwd();
            }
        }

        [DllImport("libc", EntryPoint = "kill", SetLastError = true)]
        internal static extern int Kill(int pid, int sig);

        [DllImport("libc", EntryPoint = "strerror", SetLastError = false)]
        private static extern IntPtr StrError(int errnum);


        internal static string GetLastError() => Marshal.PtrToStringAnsi(StrError(Marshal.GetLastWin32Error()));
    }
}