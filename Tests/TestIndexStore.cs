﻿using System.Collections.Generic;
using System.Linq;
using Microsoft.WindowsAzure.Storage;
using NUnit.Framework;

namespace Two10.StorageExtension.Tests
{
    [TestFixture]
    public class TestIndexStore : TestBase
    {

        private class Foo
        {
            public int Count { get; set; }
        }

        [TestFixtureTearDown]
        public void TearDown()
        {
            var indexClient = Account.CreateCloudIndexClient();
            var index = indexClient.GetIndexReference<Foo>("test");
            index.DeleteIfExists();
        }

        [TestFixtureSetUp]
        public void SetUp()
        {
            var indexClient = Account.CreateCloudIndexClient();
            var index = indexClient.GetIndexReference<Foo>("test");
            index.CreateIfNotExists();
        }

        [Test]
        public void TestIndex()
        {
            var indexClient = Account.CreateCloudIndexClient();
            var index = indexClient.GetIndexReference<Foo>("test");
            index.Put("foo", "bar");

            var item = index.Get("foo", "bar");
            Assert.AreEqual("foo", item.Key);
            Assert.AreEqual("bar", item.Value);
            Assert.IsNull(item.Metadata);

            item = index.Get("baz", "qux");
            Assert.IsNull(item);

            var items = index.Query("foo").ToArray();
            Assert.AreEqual(1, items.Length);

            index.Put("A", "B", new Foo { Count = 4 });
            var item2 = index.Get("A", "B");
            Assert.AreEqual("A", item2.Key);
            Assert.AreEqual("B", item2.Value);
            Assert.IsNotNull(item2.Metadata);
            Assert.AreEqual(4, item2.Metadata.Count);
        }

        [Test]
        public void TestDictionary()
        {
            var indexClient = Account.CreateCloudIndexClient();
            var index = indexClient.GetIndexReference("test2");
            index.CreateIfNotExists();
            index.Put("X", "Y");
            var item = index.Get("X", "Y");
            Assert.IsNotNull(item);

            var dictionary = new Dictionary<string, string>();
            dictionary.Add("BAR", "BAZ");
            index.Put("X", "Y", dictionary);
            item = index.Get("X", "Y");
            Assert.IsNotNull(item);
            Assert.AreEqual("BAZ", item.Metadata["BAR"]);

            index.Delete("X", "Y");
            item = index.Get("X", "Y");
            Assert.IsNull(item);
        }
    }
}
