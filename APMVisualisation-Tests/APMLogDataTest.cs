using APMVisualisation;
using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace APMVisualisation_Tests
{
    [TestClass]
    public class APMLogDataTest
    {
        private APMLogData log;

        [TestInitialize()]
        public void MyTestInitialize()
        {
            log = new APMLogData("test.bin");
        }

        [TestMethod]
        public void APMLogDataFirstEntry()
        {
            APMLogEntry e = log.entries[0];
            Assert.AreEqual(1, e.actions);
            Assert.AreEqual(2, e.time);
            Assert.AreEqual(3, e.apm);
        }

        [TestMethod]
        public void APMLogDataAverageAPM()
        {
            Assert.AreEqual(120, log.average_apm);
        }

        [TestMethod]
        public void APMLogDataTotalTime()
        {
            Assert.AreEqual(30, log.total_time.TotalSeconds);
        }

        [TestMethod]
        public void APMLogDataMaxAPM()
        {
            Assert.AreEqual(25, log.max_apm);
        }

        [TestMethod]
        public void APMLogEntryCount()
        {
            Assert.AreEqual(7, log.entries.Count);
        }

        [TestMethod]
        public void APMLogDataTime()
        {
            DateTime expect = new DateTime(2007, 9, 22, 13, 37, 0);
            Assert.AreEqual(expect, log.time);
        }

        [TestMethod]
        public void APMLogDataOutcome()
        {
            APMLogGameOutcome expect = APMLogGameOutcome.win;
            Assert.AreEqual(expect, log.outcome);
        }
    }
}
