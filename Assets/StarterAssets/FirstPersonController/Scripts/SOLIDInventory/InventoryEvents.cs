using System.Collections.Generic;
using System.Threading.Tasks;

namespace StarterAssets.FirstPersonController.Scripts.SOLIDInventory
{
    public static class InventoryEvents
    {
        public delegate Task InventoryUpdatedHandler(List<InventorySlots> slots);
        public static event InventoryUpdatedHandler OnInventoryUpdated;
    
        public delegate Task<int> ItemAddedHandler(Item item, int amount);
        public static event ItemAddedHandler OnItemAdded;
    
        public delegate Task<bool> ItemRemovedHandler(int amount);
        public static event ItemRemovedHandler OnItemRemoved;
        
        public delegate Task ClearInventory();
        public static event ClearInventory OnClearInventory;
        public static void InvokeInventoryUpdated(List<InventorySlots> inventorySlotsList) => OnInventoryUpdated?.Invoke(inventorySlotsList);
    
        public static async Task<int> InvokeItemAdded(Item item, int amount) { return await OnItemAdded?.Invoke(item, amount); }
        public static void InvokeItemRemoved(int amount) => OnItemRemoved?.Invoke(amount);
        public static void InvokeClearInventory() => OnClearInventory?.Invoke();
    }
}
