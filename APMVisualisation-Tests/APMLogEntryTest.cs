using APMLogIO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace APMVisualisation_Tests
{
    [TestClass()]
    public class APMLogEntryTest
    {


        private TestContext testContextInstance;
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        #region Additional test attributes
        // 
        //You can use the following additional attributes as you write your tests:
        //
        //Use ClassInitialize to run code before running the first test in the class
        //[ClassInitialize()]
        //public static void MyClassInitialize(TestContext testContext)
        //{
        //}
        //
        //Use ClassCleanup to run code after all tests in a class have run
        //[ClassCleanup()]
        //public static void MyClassCleanup()
        //{
        //}
        //
        //Use TestInitialize to run code before running each test
        //[TestInitialize()]
        //public void MyTestInitialize()
        //{
        //}
        //
        //Use TestCleanup to run code after each test has run
        //[TestCleanup()]
        //public void MyTestCleanup()
        //{
        //}
        //
        #endregion


        [TestMethod()]
        public void testEntryConstructor()
        {
            byte[] buffer = { (byte)1, (byte)0, (byte)0, (byte)0, (byte)2, (byte)0, (byte)0, (byte)0, (byte)3, (byte)0, (byte)0, (byte)0 };
            int offset = 0;
            APMLogEntry target = new APMLogEntry(buffer, offset);
            Assert.AreEqual(1, target.actions);
            Assert.AreEqual(2, target.time);
            Assert.AreEqual(3, target.apm);
        }

        [TestMethod()]
        public void testEntryConstructorWithOffset()
        {
            byte[] buffer = { (byte)1, (byte)0, (byte)0, (byte)0, (byte)2, (byte)0, (byte)0, (byte)0, (byte)3, (byte)0, (byte)0, (byte)0, (byte)4, (byte)0, (byte)0, (byte)0 };
            int offset = 4;
            APMLogEntry target = new APMLogEntry(buffer, offset);
            Assert.AreEqual(2, target.actions);
            Assert.AreEqual(3, target.time);
            Assert.AreEqual(4, target.apm);
        }

        [TestMethod()]
        public void testEntryConstructorShortBuffer()
        {
            byte[] buffer = { (byte)1, (byte)0, (byte)0, (byte)0, (byte)2, (byte)0, (byte)0, (byte)0, (byte)3 };
            try
            {
                new APMLogEntry(buffer, 0);
                Assert.Fail("Constructor with to few ints succeeded");
            }
            catch(Exception e) { };
        }
    }
}
