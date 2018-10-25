using System;
using System.Linq;
using Linux;
using Xunit;

namespace ProcessManager.Tests
{
    public class ProcessManagerTests
    {
        [Fact]
        public void ProcessManager_GetProcessInfos_Should_Return_All_Processes_Info()
        {
            var actual = Linux.ProcessManager.GetProcessInfos();
            Assert.NotEmpty(actual);
            var initProcess = actual.FirstOrDefault(_ => _.ProcessId == 1);
            Assert.NotNull(initProcess);
            Assert.Equal(1, initProcess.ProcessId);
            Assert.Equal(0, initProcess.Ruid);
            Assert.Equal(0, initProcess.Euid);
            Assert.Equal(0, initProcess.Rgid);
            Assert.Equal(0, initProcess.Egid);
            Assert.True(initProcess.StartTime > DateTime.MinValue);
            Assert.Equal("root", initProcess.UserName);
        }
        
        [Fact]
        public void ProcessManager_GetProcessInfos_Should_Return_All_Processes_Info_With_Specific_Ids()
        {
            var initProcess = Linux.ProcessManager.GetProcessInfos(new []{1}).FirstOrDefault();
            Assert.NotNull(initProcess);
            Assert.Equal(1, initProcess.ProcessId);
            Assert.Equal(0, initProcess.Ruid);
            Assert.Equal(0, initProcess.Euid);
            Assert.Equal(0, initProcess.Rgid);
            Assert.Equal(0, initProcess.Egid);
            Assert.True(initProcess.StartTime > DateTime.MinValue);
            Assert.Equal("root", initProcess.UserName);
        }

        [Fact]
        public void ProcessManager_GetProcessInfo_Should_Return_Process_Info()
        {
            var initProcess = Linux.ProcessManager.GetProcessInfo(1);
            Assert.NotNull(initProcess);
            Assert.Equal(1, initProcess.ProcessId);
            Assert.Equal(0, initProcess.Ruid);
            Assert.Equal(0, initProcess.Euid);
            Assert.Equal(0, initProcess.Rgid);
            Assert.Equal(0, initProcess.Egid);
            Assert.True(initProcess.StartTime > DateTime.MinValue);
            Assert.Equal("root", initProcess.UserName);
        }
        
        [Fact]
        public void ProcessManager_GetProcessInfo_Should_Return_Null_Process_Not_Found()
        {
            var initProcess = Linux.ProcessManager.GetProcessInfo(-1);
            Assert.Null(initProcess);
        }
        
        [Fact]
        public void ProcessManager_GetProcessInfos_Should_Return_ExecutablePath_If_Allowed()
        {
            var actual = Linux.ProcessManager.GetProcessInfos();
            Assert.NotEmpty(actual);
            var uid = Syscall.GetEffectiveUserId();
            var userProcess = actual.FirstOrDefault(_ => _.Euid == uid);
            Assert.NotNull(userProcess);
            Assert.Equal(uid, userProcess.Ruid);
            Assert.Equal(uid, userProcess.Euid);
            Assert.NotEmpty(userProcess.ExecutablePath);
        }
        
        [Fact]
        public void ProcessManager_EnumerateProcessIds_Should_Return_Pid()
        {
            var actual = Linux.ProcessManager.EnumerateProcessIds();
            Assert.NotEmpty(actual);
        }
        
        [Fact]
        public void ProcessManager_GetProcessIds_Should_Return_Pids()
        {
            var actual = Linux.ProcessManager.GetProcessIds();
            Assert.Contains(1,actual);
        }
        
        [Fact]
        public void ProcessManager_GetCmdLine_Should_Return_List_Of_Commands()
        {
            var pid = 1;
            var actual = Linux.ProcessManager.GetCmdLine(pid);
            Assert.True(actual.Count >= 1, $"Actual: {string.Join("\n",actual)}");
            Assert.Equal("/sbin/init", actual[0]);
        }
    }
}