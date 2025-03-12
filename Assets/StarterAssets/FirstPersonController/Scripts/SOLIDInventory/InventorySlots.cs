using System;
using UnityEngine;
using System.Threading.Tasks;

namespace StarterAssets.FirstPersonController.Scripts.SOLIDInventory
{
    public class InventorySlots
    {
        public Item Item { get; private set; }
        public int Amount { get; private set; }
        public GameObject Slot { get; private set; }
    
        public async Task AddItem(Item item, int amount)
        {
            Item = item;
            if(Item!=null&&Amount+amount<=Item.maxStackSize) Amount += amount;
            await Task.Yield();
        }
    
        public async Task RemoveItem(int amount)
        {
            Amount -= amount;
            if (Amount <= 0) await ClearSlot();
            await Task.Yield();
        }
        public async Task ClearSlot()
        {
            Item = null;
            Amount = 0;
            await Task.Yield();

        }
        public void SetGSlot(GameObject slot)=>Slot = slot;
        public bool IsEmpty() => Item == null;
    }
}
