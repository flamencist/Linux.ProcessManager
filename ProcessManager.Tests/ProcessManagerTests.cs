using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using Linux;
using Xunit;
using Xunit.Abstractions;

namespace ProcessManager.Tests
{
    public class ProcessManagerTests
    {
        private readonly ITestOutputHelper _testOutput;

        public ProcessManagerTests(ITestOutputHelper testOutput)
        {
            _testOutput = testOutput;
        }
        
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
        public void ProcessManager_GetProcessInfos_Should_Return_All_Processes_Info_By_Predicate()
        {
            var actual = Linux.ProcessManager.GetProcessInfos(_ => _.ProcessId == 1);
            Assert.NotEmpty(actual);
            var initProcess = actual.FirstOrDefault();
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
        public void ProcessManager_GetProcessInfos_Should_Return_All_Processes_Info_By_Predicate_And_Id()
        {
            var actual = Linux.ProcessManager.GetProcessInfos(new []{1},_ => _.ProcessId == 1);
            Assert.NotEmpty(actual);
            var initProcess = actual.FirstOrDefault();
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
        public void ProcessManager_GetProcessInfoById_Should_Return_Process_Info()
        {
            var initProcess = Linux.ProcessManager.GetProcessInfoById(1);
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
        public void ProcessManager_GetProcessInfoById_Should_Return_Null_Process_Not_Found()
        {
            var initProcess = Linux.ProcessManager.GetProcessInfoById(-1);
            Assert.Null(initProcess);
        }
        
        [Fact]
        public void ProcessManager_GetProcessInfos_Should_Return_ExecutablePath_If_Allowed()
        {
            var actual = Linux.ProcessManager.GetProcessInfos();
            Assert.NotEmpty(actual);
            var uid = Syscall.GetEffectiveUserId();
            var userProcess = actual.FirstOrDefault(_ => _.Euid == uid && _.ProcessName != "sshd");
            Assert.NotNull(userProcess);
            _testOutput.WriteLine($"{userProcess.ProcessName}:{userProcess.ProcessId}. Exe: {userProcess.ExecutablePath}. Uid: {userProcess.Euid}");
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

        [Fact]
        public void ProcessManager_Kill_Throw_Exception_If_Not_Found_Process()
        {
            var processId = Linux.ProcessManager.GetProcessIds().Max() + 10;
            var actual = Assert.Throws<Win32Exception>(() => Linux.ProcessManager.Instance.Kill(processId, 0));
            Assert.Equal("No such process", actual.Message);
            Assert.Equal(-1, actual.NativeErrorCode);
        }
        
        [Fact]
        public void ProcessManager_TryKill_Return_False_If_Not_Found_Process()
        {
            var processId = Linux.ProcessManager.GetProcessIds().Max() + 10;
            var actual = Linux.ProcessManager.Instance.TryKill(processId, 0);
            Assert.False(actual);
        }

        [Fact]
        public void ProcessManager_Kill_Send_Signal_To_Process()
        {
            var process = Process.Start(new ProcessStartInfo("bash", "-c 'exec -a sadhadxk sleep 1000000'"));
            Assert.NotNull(process);
            Linux.ProcessManager.Instance.Kill(process.Id, 9);
            var actual = Linux.ProcessManager.GetProcessInfoById(process.Id);
            Assert.Null(actual);
        }
    }
}