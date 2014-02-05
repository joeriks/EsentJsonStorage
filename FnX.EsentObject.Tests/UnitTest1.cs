using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FnX.EsentObject.Tests
{
    [TestClass]
    public class StoreTest
    {
        class Person {
            public string Id {get;set;}
            public string Name {get;set;}
        }
        [TestMethod]
        public void TestMethod1()
        {
            var s = EsentKeyValue.GetStore<Person>();
            var id = s.Set(new Person());

            Assert.IsTrue(id.Length > 8);
        }
    }
}
