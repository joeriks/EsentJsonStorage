using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FnX.EsentObject.Tests
{
    [TestClass]
    public class KeyValueTests
    {
        [TestMethod]
        public void Test()
        {
            var p = EsentKeyValue.GetStore("foo");
            p.Set("a", "b");

            var s = p.Get("a");

            Assert.AreEqual("b", s);

        }
        [TestMethod]
        public void TestWithTypedStore()
        {
            var p = EsentKeyValue.GetStore<int>("foo");
            p.Set("a", 123);

            var s = p.Get("a");

            Assert.AreEqual(123, s);

        }
        [TestMethod]
        public void TestWithStoreAndSerialize()
        {
            var p = EsentKeyValue.GetStore("foo");
            var someDate = new DateTime(2011, 11, 11);
            p.Set("a", someDate);

            var s = p.Get<DateTime>("a");

            Assert.AreEqual(someDate, s);

        }

        [TestMethod]
        public void MultipleStores()
        {
            var s1 = EsentKeyValue.GetStore("foo");
            var s2 = EsentKeyValue.GetStore("foo");
            var someDate = new DateTime(2011, 11, 11);
            s1.Set("a", someDate);

            var s = s2.Get<DateTime>("a");

            Assert.AreEqual(someDate, s);

        }

        [TestMethod]
        public void TestAdd1()
        {
            using (var p = EsentKeyValue.GetStore<int>("bar"))
            {
                p.Do("counter", t => t + 1);
            }
        }
        [TestMethod]
        public void ParallellTest1()
        {

            Parallel.Invoke(
                () => TestWithTypedStore(),
                () => TestWithTypedStore(),
                () => TestWithTypedStore());

            Assert.AreEqual(1,EsentKeyValue._dictionaries.Count);

        }

[TestMethod]
public void ParallellTest()
{
    using (var p = EsentKeyValue.GetStore<int>("bar"))
    {
        p.Set("counter", 0);

        Parallel.Invoke(
            () => TestAdd1(),
            () => TestAdd1(),
            () => TestAdd1());

        var result = p.Get("counter");
        Assert.AreEqual(1, EsentKeyValue._dictionaries.Count);

        Assert.AreEqual(3, result);
    }
}
    }
}
