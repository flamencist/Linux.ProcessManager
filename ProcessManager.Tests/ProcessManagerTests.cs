using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using Linux;
using ProcessManager.Tests.TestUtils;
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
        public void ProcessManager_GetEnvironmentVariables_Should_Return_Dictionary_Of_Variables()
        {
            var pid = Process.GetCurrentProcess().Id;
            var actual = Linux.ProcessManager.Instance.GetEnvironmentVariables(pid);
            Assert.NotNull(actual);
            Assert.NotEmpty(actual);
            Assert.Contains(actual, _=>"USER".Equals(_.Key,StringComparison.OrdinalIgnoreCase));
            _testOutput.WriteLine(string.Join(Environment.NewLine,actual.Select(_=>_.Key+":"+_.Value)));
        }
        
        [Fact]
        public void ProcessManager_GetEnvironmentVariables_Should_Filter_By_Predicate()
        {
            var pid = Process.GetCurrentProcess().Id;
            var actual = Linux.ProcessManager.Instance.GetEnvironmentVariables(pid,_=>"USER".Equals(_.Key,StringComparison.OrdinalIgnoreCase));
            Assert.NotNull(actual);
            Assert.Single(actual);
            _testOutput.WriteLine(string.Join(Environment.NewLine,actual.Select(_=>_.Key+":"+_.Value)));
        }
        
        [Fact]
        public void ProcessManager_GetEnvironmentVariable_Should_Return_Value()
        {
            var pid = Process.GetCurrentProcess().Id;
            var actual = Linux.ProcessManager.Instance.GetEnvironmentVariable(pid, "USER");
            Assert.NotNull(actual);
            Assert.NotEmpty(actual);
            _testOutput.WriteLine(actual);
        }

        [Fact]
        public void ProcessManager_Kill_Throw_Exception_If_Not_Found_Process()
        {
            var processId = Linux.ProcessManager.GetProcessIds().Max() + 1000;
            var actual = Assert.Throws<Win32Exception>(() => Linux.ProcessManager.Instance.Kill(processId, ProcessSignal.CHECK_PROCESS));
            Assert.Equal("No such process", actual.Message);
            Assert.Equal(-1, actual.NativeErrorCode);
        }
        
        [Fact]
        public void ProcessManager_TryKill_Return_False_If_Not_Found_Process()
        {
            var processId = Linux.ProcessManager.GetProcessIds().Max() + 1000;
            var actual = Linux.ProcessManager.Instance.TryKill(processId, ProcessSignal.CHECK_PROCESS);
            Assert.False(actual);
        }

        [Fact]
        public void ProcessManager_Kill_Send_Signal_To_Process()
        {
            var process = StartProcess();
            Assert.NotNull(process);
            Linux.ProcessManager.Instance.Kill(process.Id, 9);
            Waiter.Wait(()=>Linux.ProcessManager.GetProcessInfoById(process.Id)==null, TimeSpan.FromSeconds(2));
            var actual = Linux.ProcessManager.GetProcessInfoById(process.Id);
            Assert.Null(actual);
        }
        
        [Fact]
        public void ProcessManager_Kill_By_Process_Name()
        {
            {
                var process = StartProcess();
                
                var uid = Syscall.GetEffectiveUserId();
                Assert.NotNull(process);
                Linux.ProcessManager.Instance.Kill("sleep", (uint)uid, ProcessSignal.SIGKILL,e=>_testOutput.WriteLine(e.Message));
                
                Waiter.Wait(()=>Linux.ProcessManager.GetProcessInfoById(process.Id)==null, TimeSpan.FromSeconds(2));
                var actual = Linux.ProcessManager.GetProcessInfoById(process.Id);
                Assert.Null(actual);
            }
            
            {
                var process = StartProcess();
                var name = Syscall.GetPasswdByUserId(Syscall.GetEffectiveUserId()).pw_name;
                Assert.NotNull(process);
                Linux.ProcessManager.Instance.Kill("sleep", name, ProcessSignal.SIGKILL,e=>_testOutput.WriteLine(e.Message));
                Waiter.Wait(()=>Linux.ProcessManager.GetProcessInfoById(process.Id)==null, TimeSpan.FromSeconds(2));
                var actual = Linux.ProcessManager.GetProcessInfoById(process.Id);
                Assert.Null(actual);
            }
            
            {
                var process = StartProcess();
                Assert.NotNull(process);
                Linux.ProcessManager.Instance.Kill(_=>_.ProcessName == "sleep", ProcessSignal.SIGKILL, e=>_testOutput.WriteLine(e.Message));
                Waiter.Wait(()=>Linux.ProcessManager.GetProcessInfoById(process.Id)==null, TimeSpan.FromSeconds(2));
                var actual = Linux.ProcessManager.GetProcessInfoById(process.Id);
                Assert.Null(actual);
            }
        }

        [Fact]
        public void ProcessManager_Kill_By_UserName_Throw_Exception_If_Not_Found_UserName()
        {
            var username = Guid.NewGuid().ToString();
            var actual = Assert.Throws<Win32Exception>(() => Linux.ProcessManager.Instance.Kill("processName", username));
            Assert.Equal($"Not found user '{username}'",actual.Message);
        }

        private Process StartProcess()
        {
            var process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "/bin/bash",
                    Arguments = "-c \"sleep infinity \"",
                    RedirectStandardError = true,
                    RedirectStandardOutput = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                },
                EnableRaisingEvents = true
            };
            process.Start();
            process.StandardError.ReadLineAsync().ContinueWith(t => _testOutput.WriteLine(t.Result));

            Waiter.Wait(()=>Linux.ProcessManager.GetProcessInfoById(process.Id)!=null, TimeSpan.FromSeconds(2));
            return process;
        }
    }
}