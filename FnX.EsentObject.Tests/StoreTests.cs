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
        public void Gets_Id()
        {
            var s = EsentKeyValue.GetStore<Person>();
            var id = s.Set(new Person());

            Assert.IsTrue(id.Length > 8);
        }

        [TestMethod]
        public void Creates_Revision()
        {
            var s = EsentKeyValue.GetStore<Person>();
            s.Dictionary.Clear();
            var person = new Person();
            person.Name="";

            var id = s.Set(person);
            person.Id = id;
            person.Name = "some name";
            s.Set(person);

            var personOld = s.Get(id + "-1");
            Assert.AreEqual("", personOld.Name);
            var personNew = s.Get(id);
            Assert.AreEqual("some name", personNew.Name);

        }
    }
}
