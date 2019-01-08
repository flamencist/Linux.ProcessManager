using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace Linux
{
    internal static class Syscall
    {
        private const string LibC = "libc";

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

        private enum SysConfOptions
        {
            _SC_GETPW_R_SIZE_MAX = 70
        }

        [DllImport(LibC, EntryPoint = "readlink", SetLastError = true)]
        internal static extern int ReadLink(string path, byte[] buf, int bufsiz);

        [DllImport(LibC, EntryPoint = "geteuid", SetLastError = true)]
        internal static extern int GetEffectiveUserId();

        [DllImport(LibC, EntryPoint = "getpwuid_r", SetLastError = true)]
        private static extern int getpwuid_r(int uid, IntPtr pwd, IntPtr buf, long buflen, ref IntPtr result);

        [DllImport(LibC, EntryPoint = "sysconf", SetLastError = true)]
        private static extern long sysconf(int name);
        
        [DllImport(LibC, EntryPoint = "getpwnam_r", SetLastError = true)]
        private static extern int getpwnam_r(string name, IntPtr pwd, IntPtr buf, long buflen, ref IntPtr result);
        
        [DllImport(LibC, EntryPoint = "getpwent", SetLastError = true)]
        internal static extern IntPtr getpwent();
        
        [DllImport(LibC, EntryPoint = "kill", SetLastError = true)]
        internal static extern int Kill(int pid, int sig);

        [DllImport(LibC, EntryPoint = "strerror", SetLastError = false)]
        private static extern IntPtr StrError(int errnum);

        private delegate int GetPasswdDelegate(IntPtr pwd, IntPtr buf, int bufSize, ref IntPtr result);
        private static Passwd GetPasswd(GetPasswdDelegate syscallFunc)
        {
            var pwd = Marshal.AllocHGlobal(Marshal.SizeOf<Passwd>());
            var bufSize = GetBufSize();
            var buf = Marshal.AllocHGlobal(Marshal.SizeOf<char>() * bufSize);
            try
            {
                var result = IntPtr.Zero;
                var status = syscallFunc(pwd,buf,bufSize,ref result);
                if (status != 0)
                {
                    throw new Win32Exception(GetLastError());
                }

                return result == IntPtr.Zero ? new Passwd() : Marshal.PtrToStructure<Passwd>(pwd);
            }
            catch (Exception e)
            {
                Trace.WriteLine(e);
                return new Passwd();
            }
            finally
            {
                Marshal.FreeHGlobal(buf);
                Marshal.FreeHGlobal(pwd);
            }
        }
        
        internal static Passwd GetPasswdByUserId(int uid)
        {
            return GetPasswd((IntPtr pwd, IntPtr buf, int bufSize, ref IntPtr result) =>
                getpwuid_r(uid, pwd, buf, bufSize, ref result));
        }

        internal static Passwd GetPasswdByUserName(string name)
        {
            return GetPasswd((IntPtr pwd, IntPtr buf, int bufSize, ref IntPtr result) =>
                getpwnam_r(name, pwd, buf, bufSize, ref result));
        }

        private static int GetBufSize()
        {
            var bufSize = (int) sysconf((int) SysConfOptions._SC_GETPW_R_SIZE_MAX);
            if (bufSize == -1)
            {
                bufSize = 1024;
            }
            return bufSize;
        }

        internal static string GetLastError() => Marshal.PtrToStringAnsi(StrError(Marshal.GetLastWin32Error()));
    }
}