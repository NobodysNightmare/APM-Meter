using APMLogIO;
using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace APMVisualisation_Tests
{
    [TestClass]
    public class APMLogWriteTests
    {
        private APMLog log;
        private String tmp_file;

        [TestInitialize()]
        public void MyTestInitialize()
        {
            tmp_file = Path.GetTempFileName();
            File.Copy("test.bin", tmp_file, true);
            log = new APMLog(tmp_file);
        }

        [TestCleanup]
        public void MyTestCleanup()
        {
            log.Close();
        }

        [TestMethod]
        public void testWriteOutcome()
        {
            log.outcome = APMLogGameOutcome.lose;
            log = new APMLog(tmp_file);
            Assert.AreEqual(APMLogGameOutcome.lose, log.outcome);
        }
    }
}
