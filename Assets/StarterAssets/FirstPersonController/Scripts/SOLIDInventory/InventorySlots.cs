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
    
        public void AddItem(Item item, int amount)
        {
            Item = item;
            if(Item!=null&&Amount+amount<=Item.maxStackSize) Amount += amount;
        }
        public void RemoveItem(int amount)
        {
            Amount -= amount;
            if (Amount <= 0) ClearSlot();
        }
        public void ClearSlot()
        {
            Item = null;
            Amount = 0;
        }
        public void SetGSlot(GameObject slot)=>Slot = slot;
        public bool IsEmpty() => Item == null;
    }
}
