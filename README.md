# workstation-monitoring

### Overview

This repository contains a simple workstation monitoring system, consisting of:
- ServerModule
- ClientModule

ClientModule/ServerModule communication is implemented using SignalR:
- ServerModule implements `ReportHub : Hub` which contains `SendReport(string report)` method
- ClientModule connects to ServerModule and invokes its `SendReport(string report)` method

Both ClientModule and ServerModule implement their core functality as Microsoft Windows Services:
- `ClientService : BackgroundService`
- `ServerService : BackgroundService`

### How to run and test locally

1. Open *WorkstationMonitoring.sln* in Visual Studio 2022.
2. Configure how often reports will be sent from ClientModule to ServerModule in *appsettings.json* (in minutes):

<img src="https://michalantolik.blob.core.windows.net/workstation-monitoring/ReportsSendingIntervalInMinutes.png" width="800" />
  
3. Right-click "Solution 'WorkstationMonitoring'" 👉 click "Properties"
4. Configure "Multiple startup projects" like shown below:

<img src="https://michalantolik.blob.core.windows.net/workstation-monitoring/MultipleStartupProjects.png" width="800" />
  
5. Press F5 or CTRL + F5 to start the solution.
6. ClientModule and ServerModule are started.
7. You can see ongoing SignalR communication between ClientModule and ServerModule in console windows:

<img src="https://michalantolik.blob.core.windows.net/workstation-monitoring/Debugging.png" width="800" />

8. You can also check SignalR communication between ClientModule and ServerModule in their log files:
   
  - log file names contain year and month in their names

<img src="https://michalantolik.blob.core.windows.net/workstation-monitoring/LogFiles.png" width="200" />

<img src="https://michalantolik.blob.core.windows.net/workstation-monitoring/ClientLogFile.png" width="800" />

<img src="https://michalantolik.blob.core.windows.net/workstation-monitoring/ServerLogFile.png" width="800" />

### Web resources

#### Tutorial: SignalR Self-Host
- https://learn.microsoft.com/en-us/aspnet/signalr/overview/deployment/tutorial-signalr-self-host

#### Create Windows Service using BackgroundService
- https://learn.microsoft.com/en-us/dotnet/core/extensions/windows-service?pivots=dotnet-6-0

#### Logging providers in .NET
- https://learn.microsoft.com/en-us/dotnet/core/extensions/logging-providers#windows-eventlog
