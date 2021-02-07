# Linux.ProcessManager

[![Build Status](https://travis-ci.org/flamencist/Linux.ProcessManager.svg?branch=master)](https://travis-ci.org/flamencist/Linux.ProcessManager)
[![NuGet](https://img.shields.io/nuget/v/Linux.ProcessManager.svg)](https://www.nuget.org/packages/Linux.ProcessManager/)
[![Total NuGet downloads](https://img.shields.io/nuget/dt/Linux.ProcessManager?color=blue&label=downloads&logo=nuget)](https://www.nuget.org/stats/packages/Linux.ProcessManager?groupby=Version&groupby=ClientName&groupby=ClientVersion "Total NuGet downloads")

Process manager for linux. It's can retrieve processes with linux specific info (uid of owner,gid of owner, executable path ...).

Help support the project:

<a href="https://www.buymeacoffee.com/flamencist" target="_blank"><img src="https://www.buymeacoffee.com/assets/img/custom_images/orange_img.png" alt="Buy Me A Coffee" style="height: auto !important;width: auto !important;" ></a>

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

### GetProcessInfos (for processes ids)

```c#
ProcessInfo[] processInfos = ProcessManager.GetProcessInfos(new []{1,2,3});
```

### GetProcessInfos (with predicate)

```c#
ProcessInfo[] processInfos = ProcessManager.GetProcessInfos(_=>_.ProcessId == 1);
```

### GetProcessInfos (with predicate and id)

```c#
ProcessInfo[] processInfos = ProcessManager.GetProcessInfos(new []{1}, _=>_.ProcessName == "init");
```


### GetProcessInfoById

```c#
ProcessInfo[] processInfos = ProcessManager.GetProcessInfoById(1);
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

### GetEnvironmentVariables
```c#
var pid = 1000;
var envVariables = ProcessManager.Instance.GetEnvironmentVariables(processId);
Console.WriteLine(envVariables["USER"]);

```

```
some_user
```


### GetEnvironmentVariables with predicate
```c#
var pid = 1000;
var envVariables = ProcessManager.Instance.GetEnvironmentVariables(processId, _=>_.Key == "USER");
Console.WriteLine(envVariables["USER"]);

```

```
some_user
```

### GetEnvironmentVariable by name
```c#
var pid = 1000;
var value = ProcessManager.Instance.GetEnvironmentVariable(processId, "USER");
Console.WriteLine(value);

```

```
some_user
```


### Kill (by pid)
```c#
var pid = 1000;
var signal = 9;//SIGKILL
ProcessManager.Instance.Kill(processId, signal);

ProcessManager.Instance.Kill(processId, ProcessSignal.SIGKILL);

```

### TryKill
```c#
var pid = 1000000; // not existing process
var result = ProcessManager.Instance.TryKill(processId, ProcessSignal.SIGKILL);
Console.WriteLine(result);

```

```
False
```

### Kill (by process name)
```c#
ProcessManager.Instance.Kill("some_process", "username", ProcessSignal.SIGKILL, (ex)=>Console.WriteLine(ex.Message));
ProcessManager.Instance.Kill("some_process", "username"); // signal SIGTERM
ProcessManager.Instance.Kill("some_process", 1001/*user id*/, ProcessSignal.SIGKILL, (ex)=>Console.WriteLine(ex.Message));
ProcessManager.Instance.Kill("some_process", 1001/*user id*/);// signal SIGTERM

```

### Kill (by predicate)
```c#
ProcessManager.Instance.Kill((processInfo)=>processInfo.ProcessName == "some_process", ProcessSignal.SIGKILL, (ex)=>Console.WriteLine(ex.Message));
ProcessManager.Instance.Kill((processInfo)=>processInfo.ProcessName == "some_process");// signal SIGTERM

```

### License

This software is distributed under the terms of the MIT License (MIT).

### Authors

Alexander Chermyanin / [LinkedIn](https://www.linkedin.com/in/alexander-chermyanin)



Contributions and bugs reports are welcome.

