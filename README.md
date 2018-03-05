# Linux.ProcessManager

[![Build Status](https://travis-ci.org/flamencist/Linux.ProcessManager.svg?branch=master)](https://travis-ci.org/flamencist/Linux.ProcessManager)
[![NuGet](https://img.shields.io/nuget/v/Linux.ProcessManager.svg)](https://www.nuget.org/packages/Linux.ProcessManager/)

Process manager for linux. It's can retrieved processes with user\group owners.

Sample usage

```cs

var processInfos = ProcessManager.GetProcessInfos();

var initProcess = actual.FirstOrDefault(_ => _.ProcessName == "systemd"); 
Console.WriteLine($"Name: {initProcess.Name}, ProcessId: {initProcess.Pid}, Real user id: {initProcess.Ruid}, Effective user id: {initProcess.Euid} ");

```

```
Name: systemd, ProcessId: 1, Real user id: 0, Effective user id: 0
```


Contributions and bugs reports are welcome.
