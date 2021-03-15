using System.Collections.Generic;
using System.Collections.Specialized;
using InWit.Core.Collections;
using InWit.Core.Tests.Utils;
using NUnit.Framework;

namespace InWit.Core.Tests
{
    [TestFixture]
    public class ObservableContentCollectionTests
    {

        [Test]
        public void AddedItemContentChanged()
        {
            var collection = new ObservableContentCollection<TestContent> { new TestContent() };

            var changedFired = false;

            collection.CollectionContentChanged += (_, __) => changedFired = true; 

            collection[0].Data = 77.5;

            Assert.True(changedFired);
        }

        [Test]
        public void RemovedItemContentChanged()
        {
            var content = new TestContent();

            var collection = new ObservableContentCollection<TestContent> { content };

            collection.Remove(content);

            var changedFired = false;

            collection.CollectionContentChanged += (_, __) => changedFired = true;

            content.Data = 98767329;

            Assert.False(changedFired);
        }

        [Test]
        public void CreatedFromListItemContentChanged()
        {
            var collection = new ObservableContentCollection<TestContent>(new List<TestContent> { new TestContent(), new TestContent() });

            var changedFired = false;

            collection.CollectionContentChanged += (_, __) => changedFired = true; 

            collection[0].Data = 77.5;

            Assert.True(changedFired);

            collection.Add(new TestContent());

            changedFired = false;

            collection[1].Data = 77.5;

            Assert.True(changedFired);
        }


        [Test]
        public void RemoveItemWithLogic()
        {
            var point = new TestContent(3);
            var collection = new ObservableContentCollection<TestContent> { new TestContent(1), new TestContent(2), point };

            collection.CollectionChanging += (_, e) =>
                                                 {
                                                     if (e.Element == point)
                                                         e.Cancel = true;
                                                 };

            collection.RemoveAt(0);
            Assert.That(collection.Count, Is.EqualTo(2));

            collection.Remove(point);
            Assert.That(collection.Count, Is.EqualTo(2));
        }

        [Test]
        public void AddItemWithLogic()
        {
            var collection = new ObservableContentCollection<TestContent>();

            collection.CollectionChanging += (_, e) =>
                                                 {
                                                     if (e.Element.Data >= 3)
                                                         e.Cancel = true;
                                                 };

            collection.Add(new TestContent(1));
            collection.Add(new TestContent(2));
            Assert.That(collection.Count, Is.EqualTo(2));

            collection.Add(new TestContent(3));
            Assert.That(collection.Count, Is.EqualTo(2));
        }

        [Test]
        public void CanClear()
        {
            var collection = new ObservableContentCollection<TestContent> { new TestContent(1), new TestContent(2) };

            collection.Clear();

            Assert.That(collection.Count, Is.EqualTo(0));

            collection = new ObservableContentCollection<TestContent> { new TestContent(1), new TestContent(2) };

            collection.CanClear = false;
            collection.Clear();
            Assert.That(collection.Count, Is.EqualTo(2));
        }

        [Test]
        public void Refresh()
        {
            var collection = new ObservableContentCollection<TestContent> { new TestContent(1), new TestContent(2) };

            var reset = false;
            collection.CollectionChanged += (_, e) =>
                                                {
                                                    if (e.Action == NotifyCollectionChangedAction.Reset) 
                                                        reset = true;
                                                };

            collection.Refresh();

            Assert.True(reset);
        }

        [Test]
        public void TurnOffContentChanged()
        {
            var collection = new ObservableContentCollection<TestContent> { new TestContent() };

            var changedFired = false;
            collection.CollectionContentChanged += (_, __) =>  changedFired = true;

            collection.FireContentChanged = false;
            collection[0].Data = 77.5;

            Assert.False(changedFired);

            collection.FireContentChanged = true;
            collection[0].Data = 77.5;

            Assert.True(changedFired);
        }
    }
}
