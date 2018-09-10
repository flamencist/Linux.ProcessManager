# Linux.ProcessManager

[![Build Status](https://travis-ci.org/flamencist/Linux.ProcessManager.svg?branch=master)](https://travis-ci.org/flamencist/Linux.ProcessManager)
[![NuGet](https://img.shields.io/nuget/v/Linux.ProcessManager.svg)](https://www.nuget.org/packages/Linux.ProcessManager/)

Process manager for linux. It's can retrieve processes with linux specific info (uid of owner,gid of owner, executable path ...).

Sample usage

```cs

var processInfos = ProcessManager.GetProcessInfos();

var initProcess = actual.FirstOrDefault(_ => _.ProcessName == "systemd"); 
Console.WriteLine($"Name: {initProcess.ProcessName}, " +
                  $"ProcessId: {initProcess.ProcessId}, " +
                  $"Real user id: {initProcess.Ruid}, " +
                  $"Effective user id: {initProcess.Euid}, " +
                  $"User name: {initProcess.UserName}, " +
                  $"Start time: {initProcess.StartTime}, " +
                  $"Process path (exe): {initProcess.ExecutablePath}");

```

```
Name: systemd, ProcessId: 1, Real user id: 0, Effective user id: 0, User name: root, Start time: 01.03.2018, Process path (exe): /lib/systemd/systemd
```

## Api

### GetProcessInfos

```c#
ProcessInfo[] processInfos = ProcessManager.GetProcessInfos();
```

### GetProcessIds

```c#
int[] processInfos = ProcessManager.GetProcessIds();
```


### EnumerateProcessIds (lazy retrieve pid)

```c#
IEnumerable<int> processInfos = ProcessManager.EnumerateProcessIds();
var initProcessCmdLine = processInfos.Select(_=>ProcessManager.GetCmdLine(_)).First();
```

### GetCmdLine (Retrieve command line of process (exe + parameters))
```c#
var initProcessCmdLines = ProcessManager.GetCmdLine(1);
Console.WriteLine(initProcessCmdLines[0]);
```

```
/sbin/init
```

Contributions and bugs reports are welcome.
