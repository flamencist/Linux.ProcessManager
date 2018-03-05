using System.Linq;
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
            var initProcess = actual.FirstOrDefault(_ => _.ProcessName == "systemd");
            Assert.NotNull(initProcess);
            Assert.Equal("systemd",initProcess.ProcessName);
            Assert.Equal(1, initProcess.ProcessId);
            Assert.Equal(0, initProcess.Ruid);
            Assert.Equal(0, initProcess.Euid);
            Assert.Equal(0, initProcess.Rgid);
            Assert.Equal(0, initProcess.Egid);
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
    }
}