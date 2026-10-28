using System;

namespace Core
{

    public class ResourceStore
    {
        private int capacity;
        private int currentAmount;

        public ResourceStore(int capacity)
        {
            if (capacity < 0)
                throw new System.ArgumentOutOfRangeException(nameof(capacity), "Capacity cannot be negative");

            this.capacity = capacity;
            this.currentAmount = 0;
        }

        public int Capacity
        {
            get { return capacity; }
        }

        public int CurrentAmount
        {
            get { return currentAmount; }
        }

        public int AddResource(int amount)
        {
            if (amount < 0)
                throw new ArgumentOutOfRangeException(nameof(amount),"Amount cannot be negative");
            int spaceLeft = capacity - currentAmount;
            int amountToAdd = Math.Min(spaceLeft, amount);
            currentAmount += amountToAdd;
            return amountToAdd;
        }

        public int RemoveResource(int amount)
        {
            if (amount < 0)
                throw new ArgumentOutOfRangeException(nameof(amount),"Amount cannot be negative");
            int amountToRemove = Math.Min(currentAmount, amount);
            currentAmount -= amountToRemove;
            return amountToRemove;
        }
    }
}
