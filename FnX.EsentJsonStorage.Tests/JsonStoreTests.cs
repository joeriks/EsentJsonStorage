using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using Newtonsoft.Json.Linq;
using EsentJsonStorage;

namespace EsentJsonStorage.Tests
{
    [TestClass]
    public class JsonStoreTest
    {
        class Person
        {
            public string Id { get; set; }
            public string Name { get; set; }
        }
        [TestMethod]
        public void Gets_Id()
        {
            var s = Storage.GetStore();
            var id = s.Set(new Person());

            Assert.IsTrue(id.Length > 8);
        }

        [TestMethod]
        public void Creates_Revision()
        {
            var s = Storage.GetStore();
            s.Dictionary.Clear();
            var person = new Person();
            person.Name = "";

            var id = s.Set(person);
            person.Id = id;
            person.Name = "some name";
            s.Set(id, person);

            var personOld = s.Get<Person>(id, 1);
            Assert.AreEqual("", personOld.Name);
            var personNew = s.Get<Person>(id);
            Assert.AreEqual("some name", personNew.Name);

        }
        [TestMethod]
        public void Creates_Revision_Use_DeserializeId()
        {
            var s = Storage.GetStore();
            s.Dictionary.Clear();
            var person = new Person();
            person.Name = "";

            var id = s.Set(person);
            person.Id = id;
            person.Name = "some name";
            s.Set(person);

            var personOld = s.Get<Person>(id, 1);
            Assert.AreEqual("", personOld.Name);
            var personNew = s.Get<Person>(id);
            Assert.AreEqual("some name", personNew.Name);

        }
        [TestMethod]
        public void GetAll()
        {
            var s = Storage.GetStore();
            s.Dictionary.Clear();

            s.Set(new Person { Name = "Foo" });
            s.Set(new Person { Name = "Foo" });
            s.Set(new Person { Name = "Foo" });

            var p = s.GetAll<Person>();

            Assert.AreEqual("Foo", p.Skip(1).FirstOrDefault().Value.Name);

        }
        [TestMethod]
        public void GetAllAsJson()
        {
            var s = Storage.GetStore();
            s.Dictionary.Clear();

            s.Set(new Person { Name = "Foo" });
            s.Set(new Person { Name = "Foo" });
            s.Set(new Person { Name = "Foo" });

            var p = s.GetAll();

            var x = JArray.Parse(p);
            Assert.AreEqual(3, x.Count);
            Assert.AreEqual("Foo", x[1]["Name"].ToString());

        }

        [TestMethod]
        public void GetAll_NotRevisions()
        {
            using (var s = Storage.GetStore())
            {
                s.Dictionary.Clear();

                var person = new Person { Name = "Original" };
                person.Id = s.Set(person);
                person.Name = "Updated";
                s.Set(person);

                s.Set(new Person { Name = "Second" });
                s.Set(new Person { Name = "Third" });

                var p = s.GetAll<Person>();

                var getRevision = s.Get<Person>(person.Id, 1);
                Assert.AreEqual("Original", getRevision.Name);
                Assert.AreEqual("Updated", p[person.Id].Name);
                Assert.AreEqual(3, p.Count());

            }



        }

    }
}
