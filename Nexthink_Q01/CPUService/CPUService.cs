using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;

namespace Nexthink_Q01.Services
{
    public class CPUService
    {
        #region Parameters
        #region Private Parameters
        /// Name of the process to monitor [string].
        private static string _processName = "";
        /// Interval time used for the process monitoring [ms].
        private static int _intervalTime = 5000;
        /// Total time used for the process monitoring [ms].
        private static int _totalTime = 15000;


        // Parameter used for the calculation of the average CPU used
        private static double cpuSum = 0.0;

        // Parameter used for the calculation of the average memory used
        // and memory leak algorithm.
        private static double lastMemory = -1.0;
        private static double memorySum = 0.0;
        private static int memoryLeakAlgo = 0;
        private static bool memoryLeakWarning = true;

        // Parameter used for the calculation of the average handles/files used
        private static double handleSum = 0.0;
        
        // Counter fo the number of intervals done used for the average calculation
        private static int numIntervals = 0;

        #endregion

        #region Public Parameters
        public string ProcessName {
            get { return _processName; }
            set { _processName = value; }
        }
        public int TotalTime {
            get { return _totalTime; }
            set { _totalTime = value; }
        }
        public int IntervalTime {
            get { return _intervalTime; }
            set { _intervalTime = value; }
        }
        #endregion
        #endregion

        #region Constructors
        public CPUService() {
        }

        public CPUService(string procName, int intTime, int totTime) {
            ProcessName = procName;
            IntervalTime = intTime;
            TotalTime = totTime;
        }
        #endregion

        #region Functions
        public void RunProcessMonitoring() {
            if (_processName == "") {
                throw new System.ArgumentException("Missing process name!");
            }
            if (_totalTime < _intervalTime) {
                throw new System.ArgumentException("Interval time greater than total time!");
            }

            if (!IsProcessRunning()) {
                throw new System.ArgumentException("Process not running!");
            }

            Stopwatch watch = new Stopwatch();
            watch.Start();

            numIntervals = 0;
            while (watch.ElapsedMilliseconds < _totalTime)
            {
                GetProcessData();
                numIntervals++;
            }
        }

        /// <summary>
        /// Function that prints the results at the end of the program
        /// </summary>
        public void PrintResults() {
            if (lastMemory < 0) {
                throw new System.Exception("Run monitoring first!");
            }

            double avgCPU = Math.Round(cpuSum/numIntervals*100)/100;
            double avgMemory = Math.Round(memorySum/numIntervals);
            double avgHandle = Math.Round(handleSum/numIntervals);

            Console.WriteLine("");
            Console.WriteLine("Analysis of process '" + _processName + "':");
            Console.WriteLine(" - Avg CPU% " + avgCPU);
            Console.WriteLine(" - Avg Memory " + avgMemory + " bytes ");
            Console.WriteLine(" - Avg Handles/Files " + avgHandle);
            Console.WriteLine("");
        }


        /// <summary>
        /// Function checks if there is a possible memory leak.
        /// </summary>
        public void CheckMemoryLeak() {
            
            if (memoryLeakAlgo > 0)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("Warning: Memory usage increased more often than it decreased!!!");
                Console.ForegroundColor = ConsoleColor.White;
            } else if (memoryLeakWarning) {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("Warning: Memory usage never decreased, possible memory leak detected!!!");
                Console.ForegroundColor = ConsoleColor.White;
            } else {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("No memory leak detected.");
                Console.ForegroundColor = ConsoleColor.White;
            }
            Console.WriteLine("");
        }


        /// <summary>
        /// Function that gets the data from the selected process.
        /// </summary>
        public void GetProcessData()
        {
            // If there are more than one process with the same name, 
            // it will choose the first one in the list.
            Process p = Process.GetProcessesByName(_processName)[0];

            // CPU percentage is calculated on a certain time (interval time)
            var startTime = DateTime.UtcNow;
            var startCpuUsage = p.TotalProcessorTime;

            System.Threading.Thread.Sleep(_intervalTime);

            var endTime = DateTime.UtcNow;
            var endCpuUsage = p.TotalProcessorTime;

            var cpuUsedMs = (endCpuUsage - startCpuUsage).TotalMilliseconds;
            var totalMsPassed = (endTime - startTime).TotalMilliseconds;

            var cpuUsageTotal = cpuUsedMs / (Environment.ProcessorCount * totalMsPassed) * 100;
            cpuSum += cpuUsageTotal;
            
            // Gathering of the mermory usage
            var privateMemoryUsage = p.WorkingSet64;
            memorySum += privateMemoryUsage;

            // Gathering of the handle/files used
            var handleCountUsage = p.HandleCount;
            handleSum += handleCountUsage;

            // Check how many times the memory increases vs decreases
            if (lastMemory >= privateMemoryUsage) { 
                // If memory decreased once, then flag it
                memoryLeakWarning = false;
                memoryLeakAlgo--;
            } else {
                memoryLeakAlgo++;
            }
            lastMemory = privateMemoryUsage;

            /* Used for debugging to check live the values gathered
            Console.WriteLine(_processName + " - " + cpuUsageTotal + " CPU% " + 
            " - Memory usage " + endPrivateMemoryUsage + " bytes " +
            " - Handles/Files " + endHandleCountUsage);
            */
        }

        /// <summary>
        /// Function that displays the list of process currently running.
        /// </summary>
        public void ListAllRunningProcess() {
            Process[] localAll = Process.GetProcesses();
            string[] processRunning = new string[1000];
            int n = 0;
            foreach(Process proc in localAll) {
                if (n >= 1000) {
                    //No more than 1000 process displayed
                    break;
                }
                if (proc.ProcessName != "") {
                    processRunning[n] = proc.ProcessName;
                    n++;
                }
            }

            // Makes sure no duplicates are displayed.
            string[] uniqueProcessRunning = processRunning.Distinct().ToArray();
            foreach(var item in uniqueProcessRunning)
            {
                if (item == null) {
                    break;
                }
                Console.WriteLine(" - "+item.ToString());
            }
            Console.WriteLine("");
        }


        /// <summary>
        /// Function that returns true if the currently selected process 
        /// is running and returns false otherwise.
        /// </summary>
        public bool IsProcessRunning()
        {
            // Get the list of all processes running with that name
            Process[] localByName = Process.GetProcessesByName(_processName);

            // Checks if the returned array is empty or not
            if (localByName.Length == 0) {
                // Not process with that name
                return false;
            } else {
                // At least one process with that name found
                return true;
            }
        }
        #endregion
    }
}