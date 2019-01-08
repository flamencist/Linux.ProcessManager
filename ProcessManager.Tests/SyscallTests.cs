using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Linux;
using Xunit;

namespace ProcessManager.Tests
{
    public class SyscallTests
    {
        [Fact]
        public void GetPasswdByUserId_Should_Return_Passwd_Info()
        {
            var actual = Syscall.GetPasswdByUserId(0);
            Assert.Equal("root",actual.pw_name);
            Assert.Equal("root",actual.pw_gecos);
            Assert.Equal("/root",actual.pw_dir);
            Assert.Equal("x",actual.pw_passwd);
            Assert.Equal("/bin/bash",actual.pw_shell);
            Assert.Equal(0u,actual.pw_uid);
            Assert.Equal(0u,actual.pw_gid);
        }
        
        [Fact]
        public void GetPasswdByUserId_Should_Return_Empty_If_Passwd_Not_Found()
        {
            var expected = new Syscall.Passwd();
            var actual = Syscall.GetPasswdByUserId(-1);
            Assert.Equal(expected.pw_uid,actual.pw_uid);
            Assert.Equal(expected.pw_gid,actual.pw_gid);
            Assert.Equal(expected.pw_gecos,actual.pw_gecos);
            Assert.Equal(expected.pw_dir,actual.pw_dir);
            Assert.Equal(expected.pw_name,actual.pw_name);
            Assert.Equal(expected.pw_passwd,actual.pw_passwd);
            Assert.Equal(expected.pw_shell,actual.pw_shell);
        }
        
        [Fact]
        public void GetPasswdByUserName_Should_Return_Passwd_Info()
        {
            var actual = Syscall.GetPasswdByUserName("root");
            Assert.Equal("root",actual.pw_name);
            Assert.Equal("root",actual.pw_gecos);
            Assert.Equal("/root",actual.pw_dir);
            Assert.Equal("x",actual.pw_passwd);
            Assert.Equal("/bin/bash",actual.pw_shell);
            Assert.Equal(0u,actual.pw_uid);
            Assert.Equal(0u,actual.pw_gid);
        }
        
        [Fact]
        public void GetPasswdByUserName_Should_Return_Empty_If_Passwd_Not_Found()
        {
            var expected = new Syscall.Passwd();
            var actual = Syscall.GetPasswdByUserName(Guid.NewGuid().ToString());
            Assert.Equal(expected.pw_uid,actual.pw_uid);
            Assert.Equal(expected.pw_gid,actual.pw_gid);
            Assert.Equal(expected.pw_gecos,actual.pw_gecos);
            Assert.Equal(expected.pw_dir,actual.pw_dir);
            Assert.Equal(expected.pw_name,actual.pw_name);
            Assert.Equal(expected.pw_passwd,actual.pw_passwd);
            Assert.Equal(expected.pw_shell,actual.pw_shell);
        }

        
        
        [Fact]
        public void GetPasswdByUserId_Should_Retrieve_Passwd_Thread_Safe()
        {
            var simpleUserId = GetSimpleUserId();
            var rootId = 0;
            var expected = Syscall.GetPasswdByUserId(simpleUserId);
            var rootExpected = Syscall.GetPasswdByUserId(rootId);

            var tasksCount = 1024;
            var tasks = new Task<KeyValuePair<int,Syscall.Passwd>>[tasksCount];
            for (var i = 0; i < tasksCount; i++)
            {
                tasks[i] = i%2 == 0
                    ? new Task<KeyValuePair<int,Syscall.Passwd>>(() => new KeyValuePair<int,Syscall.Passwd>(simpleUserId,Syscall.GetPasswdByUserId(simpleUserId)))
                    :new Task<KeyValuePair<int,Syscall.Passwd>>(() =>  new KeyValuePair<int,Syscall.Passwd>(rootId,Syscall.GetPasswdByUserId(rootId)));
            }
            tasks.ForEach(_=>_.Start());
            var actual = Task.WhenAll(tasks).Result;

            var root = actual.Where(_ => _.Key == rootId);
            var simple = actual.Where(_ => _.Key == simpleUserId);
            Assert.All(root, _ => AssertEqual(rootExpected, _.Value));
            Assert.All(simple, _ => AssertEqual(expected, _.Value));
        }
        
        [Fact]
        public void GetPasswdByUserName_Should_Retrieve_Passwd_Thread_Safe()
        {
            var simpleUserId = GetSimpleUserId();
            var rootId = 0;
            var expected = Syscall.GetPasswdByUserId(simpleUserId);
            var rootExpected = Syscall.GetPasswdByUserId(rootId);
            
            var tasksCount = 1024;
            var tasks = new Task<KeyValuePair<string,Syscall.Passwd>>[tasksCount];
            for (var i = 0; i < tasksCount; i++)
            {
                tasks[i] = i%2 == 0
                    ? new Task<KeyValuePair<string,Syscall.Passwd>>(() => new KeyValuePair<string,Syscall.Passwd>(expected.pw_name,Syscall.GetPasswdByUserName(expected.pw_name)))
                    :new Task<KeyValuePair<string,Syscall.Passwd>>(() =>  new KeyValuePair<string,Syscall.Passwd>(rootExpected.pw_name,Syscall.GetPasswdByUserName(rootExpected.pw_name)));
            }
            tasks.ForEach(_=>_.Start());
            var actual = Task.WhenAll(tasks).Result;

            var root = actual.Where(_ => _.Key == rootExpected.pw_name);
            var simple = actual.Where(_ => _.Key == expected.pw_name);
            Assert.All(root, _ => AssertEqual(rootExpected, _.Value));
            Assert.All(simple, _ => AssertEqual(expected, _.Value));
        }


        private static void AssertEqual(Syscall.Passwd expected, Syscall.Passwd actual)
        {
            Assert.Equal(expected.pw_uid,actual.pw_uid);
            Assert.Equal(expected.pw_gid,actual.pw_gid);
            Assert.Equal(expected.pw_gecos,actual.pw_gecos);
            Assert.Equal(expected.pw_dir,actual.pw_dir);
            Assert.Equal(expected.pw_name,actual.pw_name);
            Assert.Equal(expected.pw_passwd,actual.pw_passwd);
            Assert.Equal(expected.pw_shell,actual.pw_shell);
        }

        private static int GetSimpleUserId()
        {
            var simpleUserId = 0;
            IntPtr ptr;
            while ((ptr = Syscall.getpwent()) != IntPtr.Zero)
            {
                var pwd = Marshal.PtrToStructure<Syscall.Passwd>(ptr);
                if (pwd.pw_uid != 0)
                {
                    simpleUserId = (int) pwd.pw_uid;
                    break;
                }
            }

            return simpleUserId;
        }
    }
}