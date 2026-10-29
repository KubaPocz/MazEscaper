using Core;
using NUnit.Framework;

namespace Tests
{
    public class ResourceStoreTests
    {
        [Test]
        public void CreatedResourceStore_HasCorrectCapacity()
        {
            var store = new ResourceStore(400);
            Assert.AreEqual(400, store.Capacity);
        }

        [Test]
        public void CreatedResourceStore_HasCorrectCurrentAmount()
        {
            var store = new ResourceStore(400);
            Assert.AreEqual(0, store.CurrentAmount);
        }

        [Test]
        public void AddResource_TooEmpty_IncreasesCurrentAmountAndReturnsCorrectAmount()
        {
            var store = new ResourceStore(400);
            Assert.AreEqual(50, store.AddResource(50));
            Assert.AreEqual(50, store.CurrentAmount);
        }

        [Test]
        public void AddResources_TooManyTooEmpty_ReturnsOnlyAvailableAmountAndDoesNotExceedCapacity()
        {
            var store = new ResourceStore(400);
            Assert.AreEqual(400, store.AddResource(500));
            Assert.AreEqual(400, store.CurrentAmount);
        }

        [Test]
        public void AddResources_TooManyTooPartlyFilled_ReturnsOnlyAvailableAmountAndDoesNotExceedCapacity()
        {
            var store = new ResourceStore(400);
            store.AddResource(350);
            Assert.AreEqual(50, store.AddResource(100));
            Assert.AreEqual(400, store.CurrentAmount);
        }
    
        [Test]
        public void AddResources_ToFull_ReturnsZeroAndDoesNotExceedCapacity()
        {
            var store = new ResourceStore(400);
            store.AddResource(400);
            Assert.AreEqual(0, store.AddResource(50));
            Assert.AreEqual(400, store.CurrentAmount);
        }

        [Test]
        public void RemoveResource_ReturnsCorrectAmount()
        {
            var store = new ResourceStore(400);
            store.AddResource(50);
            Assert.AreEqual(30, store.RemoveResource(30));
            Assert.AreEqual(20, store.CurrentAmount);
        }
        [Test]
        public void RemoveResource_TooMany_ReturnsOnlyAvailableAmount()
        {
            var store = new ResourceStore(400);
            store.AddResource(50);
            Assert.AreEqual(50, store.RemoveResource(80));
            Assert.AreEqual(0, store.CurrentAmount);
        }
        [Test]
        public void RemoveResource_FromEmpty_ReturnsZero()
        {
            var store = new ResourceStore(400);
            Assert.AreEqual(0, store.RemoveResource(80));
            Assert.AreEqual(0, store.CurrentAmount);
        }
        [Test]
        public void ResourcesAreIndependentBetweenInstances()
        {
            var storeA = new ResourceStore(400);
            var storeB = new ResourceStore(400);
            storeA.AddResource(50);
            Assert.AreEqual(50, storeA.CurrentAmount);
            Assert.AreEqual(0, storeB.CurrentAmount);
        }
    }
}
