using NUnit.Framework;
using System;
using System.Linq;
using USBTerminal.Core.Utils;

namespace USBTerminal.Core.Tests
{
    public class Tests
    {
        [Test]
        public void CanDetectAddedItems()
        {
            // Arrange
            var listA = ChangesTrackerMockups.GetAList(8);
            var listB = ChangesTrackerMockups.GetBList(10);
            var changesTracker = new ChangesTracker<A, B>(listA, listB, AreEqual);

            // Act
            // Assert

            Assert.AreEqual(2, changesTracker.AddedItems.Count());
            Assert.AreEqual(0, changesTracker.RemovedItems.Count());
            Assert.AreEqual(8, changesTracker.UpdatedItems.Count());
        }

        [Test]
        public void CanDetectRemovedItems()
        {
            // Arrange
            var listA = ChangesTrackerMockups.GetAList(10);
            var listB = ChangesTrackerMockups.GetBList(8);
            var changesTracker = new ChangesTracker<A, B>(listA, listB, AreEqual);

            // Act
            // Assert

            Assert.AreEqual(0, changesTracker.AddedItems.Count());
            Assert.AreEqual(2, changesTracker.RemovedItems.Count());
            Assert.AreEqual(8, changesTracker.UpdatedItems.Count());
        }


        [Test]
        public void CanDetectUpdatedItems()
        {
            // Arrange
            var listA = ChangesTrackerMockups.GetAList(10);
            var listB = ChangesTrackerMockups.GetBList(10);
            var changesTracker = new ChangesTracker<A, B>(listA, listB, AreEqual);

            // Act
            // Assert

            Assert.AreEqual(0, changesTracker.AddedItems.Count());
            Assert.AreEqual(0, changesTracker.RemovedItems.Count());
            Assert.AreEqual(10, changesTracker.UpdatedItems.Count());
        }

        [Test]
        public void AddAll()
        {
            // Arrange
            var listA = ChangesTrackerMockups.GetAList(0);
            var listB = ChangesTrackerMockups.GetBList(10);
            var changesTracker = new ChangesTracker<A, B>(listA, listB, AreEqual);

            // Act
            // Assert

            Assert.AreEqual(10, changesTracker.AddedItems.Count());
            Assert.AreEqual(0, changesTracker.RemovedItems.Count());
            Assert.AreEqual(0, changesTracker.UpdatedItems.Count());
        }


        [Test]
        public void RemoveAll()
        {
            // Arrange
            var listA = ChangesTrackerMockups.GetAList(10);
            var listB = ChangesTrackerMockups.GetBList(0);
            var changesTracker = new ChangesTracker<A, B>(listA, listB, AreEqual);

            // Act
            // Assert

            Assert.AreEqual(0, changesTracker.AddedItems.Count());
            Assert.AreEqual(10, changesTracker.RemovedItems.Count());
            Assert.AreEqual(0, changesTracker.UpdatedItems.Count());
        }

        [Test]
        public void AddRemoveAndUpdate()
        {
            // Arrange
            var listA = ChangesTrackerMockups.GetAList(10); // ids 1-10
            var listB = ChangesTrackerMockups.GetBList(11)  // ids 1-11
                .Where(b => b.Iddd != 7); // Exclude element means it will be delete
            var changesTracker = new ChangesTracker<A, B>(listA, listB, AreEqual);

            // Assert

            Assert.AreEqual(1, changesTracker.AddedItems.Count()); // b.id = 11
            Assert.AreEqual(1, changesTracker.RemovedItems.Count()); // b.id 7
            Assert.AreEqual(9, changesTracker.UpdatedItems.Count()); // all a.id == b.iddd
        }

        private bool AreEqual(A a, B b)
        {
            if (a == null && b == null)
                return true;
            if (a == null || b == null)
                return false;
            return a.Id == b.Iddd;
        }
    }
}