using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using EsentJsonStorage;

namespace EsentJsonStorage.Tests
{
    [TestClass]
    public class FileTests
    {
        [TestMethod]
        public void ExportImportTest()
        {
            var p = Storage.GetDictionary();

            p.Clear();
            p.Add("foo", "bar");
            p.Add("baz", "bang");

            Assert.AreEqual(2, p.Count);

            p.Export("file.json");
            p.Clear();

            Assert.AreEqual(0, p.Count);
            p.Import("file.json");

            Assert.AreEqual(2, p.Count);
            Assert.AreEqual("bar", p["foo"]);
            Assert.AreEqual("bang", p["baz"]);

        }


        public class SomeType
        {
            public string Id { get; set; }
            public string Value { get; set; }
            public List<string> Values { get; set; }
        }

        [TestMethod]
        public void SerializeFileTest()
        {
            var item1 = new SomeType { Id = "1", Value = "A", Values = new List<string> { "aaa", "bbb", "ccc" } };
            var item2 = new SomeType { Id = "2", Value = "B", Values = new List<string> { "ddd", "eee", "fff" } };

            var store = Storage.GetStore();

            store.Dictionary.Clear();
            store.Set(item1);
            store.Set(item2);
            store.Dictionary.Export("file.json");
            store.Dictionary.Clear();
            Assert.AreEqual(0, store.Dictionary.Count);
            store.Dictionary.Import("file.json");

            Assert.AreEqual(2, store.Dictionary.Count);

            var fetchedItem1 = store.Get<SomeType>(item1.Id);
            var fetchedItem2 = store.Get<SomeType>(item2.Id);

            Assert.AreEqual(item1.Values[2], fetchedItem1.Values[2]);
            Assert.AreEqual(item2.Values[2], fetchedItem2.Values[2]);

        }

    }
}
