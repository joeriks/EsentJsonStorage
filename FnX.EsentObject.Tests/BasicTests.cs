using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace FnX.EsentObjectTests
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void Test_Filename_AndKey()
        {

            var esentStore = new EsentStore<int>("my esent folder name");

            // init object from Esent Store
            var x1 = esentStore.Get("my object key");

            // other value
            var x2 = esentStore.Get("my object key");          

            Assert.AreEqual(x1.Value, x2.Value);

            x1.Value = 999;

            // the EsentObject wraps a call to Esent, therefore these are still equal
            Assert.AreEqual(x1.Value, x2.Value);


            // but these should not be equal as the folders are not the same
            var esentStore2 = new EsentStore<int>("my other esent folder name");
            var x3 = esentStore2.Get("my object key");
            Assert.AreNotEqual(x1.Value, x3.Value);


        }

        [TestMethod]
        public void Test_Only_Key()
        {

            // init object from default Esent Store with only key
            var x1 = new EsentObject<int>("my key");

            // other value
            var x2 = new EsentObject<int>("my key");

            Assert.AreEqual(x1.Value, x2.Value);

            x1.Value = 999;

            // the EsentObject wraps a call to Esent, therefore these are still equal
            Assert.AreEqual(x1.Value, x2.Value);

        }


        [TestMethod]
        public void Test_No_Key()
        {

            // init object from default Esent Store with only key
            var x1 = new EsentObject<int>();

            // other value
            var x2 = new EsentObject<int>();

            Assert.AreEqual(x1.Value, x2.Value);

            x1.Value = 999;

            // the EsentObject wraps a call to Esent, therefore these are still equal
            Assert.AreEqual(x1.Value, x2.Value);

        }
        [TestMethod]
        public void Test_List()
        {

            // init object from default Esent Store with only key
            var x1 = new EsentObject<List<string>>();

            // not that a better way to store a list is by using
            // var store = new EsentStore<string>("my esent folder name");

            if (x1.Value==null) x1.Value = new List<string>();

            var initialcount = x1.Value.Count;

            x1.Do(l => l.Add("foo"));

            Assert.AreEqual(x1.Value.Count,initialcount+1);


        }

        [TestMethod]
        public void Test_DifferentTypes_Same_Store()
        {

            var esentStore = new EsentStore(); // default store

            // init object from Esent Store
            var x1 = esentStore.Get<int>("my int");
            x1.Value = 999;

            // other value
            var x2 = esentStore.Get<string>("my string");
            x2.Value = "999";

            Assert.AreEqual(x1.Value, 999);
            Assert.AreEqual(x2.Value, "999");



        }

    }
}
