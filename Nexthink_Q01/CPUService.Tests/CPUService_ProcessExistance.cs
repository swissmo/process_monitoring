using NUnit.Framework;
using Nexthink_Q01.Services;

namespace Nexthink_Q01.UnitTests.Services
{
    [TestFixture]
    public class CPUService_ProcessExistance
    {
        CPUService cpuTest = null;

        [SetUp]
        public void startTest()
        {
            cpuTest = new CPUService();
        }

        [Test]
        public void processNotRunning(){
            cpuTest.ProcessName = "bla";

            Assert.IsFalse(cpuTest.IsProcessRunning());
        }

        [Test]
        public void processRunning(){
            cpuTest.ProcessName = "dotnet";

            Assert.IsFalse(!cpuTest.IsProcessRunning());
        }

        [Test]
        public void missingProcessName(){
            cpuTest.ProcessName = "";

            var ex = Assert.Throws<System.ArgumentException>(cpuTest.RunProcessMonitoring);
            Assert.That(ex.Message, Is.EqualTo("Missing process name!"));
        }

        [Test]
        public void processNotRunningWhenRunningMonitoring(){
            cpuTest.ProcessName = "bla";
            
            var ex = Assert.Throws<System.ArgumentException>(cpuTest.RunProcessMonitoring);
            Assert.That(ex.Message, Is.EqualTo("Process not running!"));
        }

        [Test]
        public void printResultsBeforeRunningMonitoring(){
            cpuTest.ProcessName = "dotnet";
            cpuTest.IntervalTime = 400;
            cpuTest.TotalTime = 4000;
            
            var ex = Assert.Throws<System.Exception>(cpuTest.PrintResults);
            Assert.That(ex.Message, Is.EqualTo("Run monitoring first!"));
        }

        [Test]
        public void greaterIntervalTimeThanTotalTime(){
            cpuTest.ProcessName = "dotnet";
            cpuTest.IntervalTime = 4000;
            cpuTest.TotalTime = 400;
            
            var ex = Assert.Throws<System.ArgumentException>(cpuTest.RunProcessMonitoring);
            Assert.That(ex.Message, Is.EqualTo("Interval time greater than total time!"));
        }
    }
}