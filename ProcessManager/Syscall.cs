using System;
using System.Runtime.InteropServices;

namespace Linux
{
    internal static class Syscall
    {
        [DllImport("libc",EntryPoint="readlink", SetLastError = true)]
        internal static extern int ReadLink(string path, byte[] buf, int bufsiz);
        
        [DllImport("libc", EntryPoint="geteuid", SetLastError = false)]
        internal static extern int GetEffectiveUserId();
        
        [DllImport("libc", EntryPoint="strerror", SetLastError = false)]
        private static extern IntPtr StrError(int errnum);

        
        internal static string GetLastError() => Marshal.PtrToStringAnsi(StrError(Marshal.GetLastWin32Error()));
        
        
    }
}