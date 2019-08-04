using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace Nexthink_Q01.Services
{
    class Program
    {

        private static CPUService processTest;
         

        /// <summary>
        /// Main function
        /// </summary>
        /// <param name="args">
        /// String array of the given parameters:
        ///   - Index 00: Name of the process to monitor.
        ///   - Index 01: Interval time used for the monitoring (default = 5000ms).
        ///   - Index 02: Total time used for the monitoring (default = 15000ms).
        /// </param>
        /// <remarks>
        /// If no arguments are given, the list of process will be displayed.
        /// If no interval time or total time is given, default values will be used.
        /// </remarks>
        public static void Main(string[] args)
        {
            processTest = new CPUService();

            if (args.Length == 0)
            {
                Console.ForegroundColor = ConsoleColor.Blue;
                Console.WriteLine("Please give arguments [Process Name, Interval Time, Total Time]");
                Console.WriteLine("Here are the currently running Processes:");
                Console.ForegroundColor = ConsoleColor.DarkCyan;
                processTest.ListAllRunningProcess();
                Console.ForegroundColor = ConsoleColor.White;
                return;
            }

            parseArgs(args);

            processTest.RunProcessMonitoring();

            processTest.PrintResults();
            
            processTest.CheckMemoryLeak();
        }


        /// <summary>
        /// Function that parses the given arguments
        /// </summary>
        private static void parseArgs(string[] args) {
            if (args.Length >= 1)
            {
                processTest.ProcessName = args[0];
            }
            if (args.Length >= 2)
            {
                processTest.IntervalTime = Int32.Parse(args[1]);
            }
            if (args.Length == 3)
            {
                processTest.TotalTime = Int32.Parse(args[2]);
            }
        }
    }
}