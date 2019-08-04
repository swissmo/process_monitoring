# process_monitoring
Monitors a process and outputs the CPU % usage, memory usage and handle/file usage.

## Info
 - Code developped under [Visual Studio Code](https://code.visualstudio.com/download)
 - [.Net Core SDK](https://dotnet.microsoft.com/download) also needed
 - Testing done using Nunit
 
## Usage
 - In a terminal, go the the Nexthink_Q01/CPUService/ folder
 - Run the following command:
    _dotnet run -- “**PROCESS_NAME**” “**INTERVAL_TIME**” “**TOTAL_TIME**”_
        Where :
            **PROCESS_NAME** is the name of the process to monitor.
            **INTERVAL_TIME** is the number of milliseconds to be used for the interval time.
            **TOTAL_TIME** is the time the monitoring should run for.
      
    Examples:
        _dotnet run -- “dotnet” “100” “4000”_
            This will run the monitoring of the process dotnet and use the interval time of 100ms and run for 4 seconds (4000ms).
        _dotnet run_
            This will display the currently running processes.
