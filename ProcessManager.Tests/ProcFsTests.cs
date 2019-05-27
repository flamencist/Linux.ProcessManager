using System;
using Linux;
using Xunit;
using Xunit.Abstractions;
using System.IO;

namespace ProcessManager.Tests
{
    public class ProcFsTests
    {
        private readonly ITestOutputHelper _testOutput;

        public ProcFsTests(ITestOutputHelper testOutput)
        {
            _testOutput = testOutput;
        }
        
        [Theory]
        [InlineData("empty","")]
        [InlineData("ubuntu-18.04","Name:\tcat\nUmask:\t0022\nState:\tR (running)\nTgid:\t13278\nNgid:\t0\nPid:\t13278\nPPid:\t13234\nTracerPid:\t0\nUid:\t1002\t1002\t1002\t1002\nGid:\t1002\t1002\t1002\t1002\nFDSize:\t256\nGroups:\t1002 \nNStgid:\t13278\nNSpid:\t13278\nNSpgid:\t13278\nNSsid:\t13234\nVmPeak:\t    8180 kB\nVmSize:\t    8180 kB\nVmLck:\t       0 kB\nVmPin:\t       0 kB\nVmHWM:\t     764 kB\nVmRSS:\t     764 kB\nRssAnon:\t      64 kB\nRssFile:\t     700 kB\nRssShmem:\t       0 kB\nVmData:\t     316 kB\nVmStk:\t     132 kB\nVmExe:\t      28 kB\nVmLib:\t    1612 kB\nVmPTE:\t      52 kB\nVmSwap:\t       0 kB\nHugetlbPages:\t       0 kB\nCoreDumping:\t0\nTHP_enabled:\t1\nThreads:\t1\nSigQ:\t8/31419\nSigPnd:\t0000000000000000\nShdPnd:\t0000000000000000\nSigBlk:\t0000000000000000\nSigIgn:\t0000000000000000\nSigCgt:\t0000000000000000\nCapInh:\t0000000000000000\nCapPrm:\t0000000000000000\nCapEff:\t0000000000000000\nCapBnd:\t0000003fffffffff\nCapAmb:\t0000000000000000\nNoNewPrivs:\t0\nSeccomp:\t0\nSpeculation_Store_Bypass:\tthread vulnerable\nCpus_allowed:\tf\nCpus_allowed_list:\t0-3\nMems_allowed:\t00000000,00000000,00000000,00000000,00000000,00000000,00000000,00000000,00000000,00000000,00000000,00000000,00000000,00000000,00000000,00000000,00000000,00000000,00000000,00000000,00000000,00000000,00000000,00000000,00000000,00000000,00000000,00000000,00000000,00000000,00000000,00000001\nMems_allowed_list:\t0\nvoluntary_ctxt_switches:\t2\nnonvoluntary_ctxt_switches:\t1\n")]
        [InlineData("wsl debian9","Name:\tbash\nState:\tS\t(sleeping)\nTgid:\t67\nPid:\t67\nPPid:\t66\nTracerPid:\t0\nUid:\t1000\t1000\t1000\t1000\nGid:\t1000\t1000\t1000\t1000\nFDSize:\t4\nGroups:\nVmPeak:\t0\tkB\nVmSize:\t14608\tkB\nVmLck:\t0\tkB\nVmHWM:\t0\tkB\nVmRSS:\t3272\tkB\nVmData:\t0\tkB\nVmStk:\t0\tkB\nVmExe:\t1024\tkB\nVmLib:\t0\tkB\nVmPTE:\t0\tkB\nThreads:\t1\nSigQ:\t0/0\nSigPnd:\t0000000000000000\nShdPnd:\t0000000000000000\nSigBlk:\t0000000000000000\nSigIgn:\t0000000000000000\nSigCgt:\t0000000000000000\nCapInh:\t0000000000000000\nCapPrm:\t0000000000000000\nCapEff:\t0000000000000000\nCapBnd:\t0000001fffffffff\nCpus_allowed:\t00000001\nCpus_allowed_list:\t0\nMems_allowed:\t1\nMems_allowed_list:\t0\nvoluntary_ctxt_switches:\t150\nnonvoluntary_ctxt_switches:\t545")]
        public void TryParseStatusFile(string distributive, string fileContent)
        {
            _testOutput.WriteLine($"Check status file of {distributive}");
            var statusFilePath = Path.Combine(Directory.GetCurrentDirectory(),Guid.NewGuid().ToString());
            using (var sw = File.CreateText(statusFilePath))
            {
                sw.Write(fileContent);
            } 
            
            var result = ProcFs.TryParseStatusFile(statusFilePath, out var actual, new ReusableTextReader());
            Assert.True(result);
        }
    }
}