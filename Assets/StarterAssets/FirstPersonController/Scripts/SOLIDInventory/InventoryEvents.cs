using System.Collections.Generic;
using System.Threading.Tasks;

namespace StarterAssets.FirstPersonController.Scripts.SOLIDInventory
{
    public static class InventoryEvents
    {
        public delegate Task InventoryUpdatedHandler();
        public static event InventoryUpdatedHandler OnInventoryUpdated;
    
        public delegate Task<int> ItemAddedHandler(Item item, int amount);
        public static event ItemAddedHandler OnItemAdded;
    
        public delegate Task<bool> ItemRemovedHandler(int amount);
        public static event ItemRemovedHandler OnItemRemoved;
        
        public delegate Task ClearInventory();
        public static event ClearInventory OnClearInventory;
        public delegate void slotsItemChanged();
        public static event slotsItemChanged OnSlotsItemChanged;
        public static void InvokeInventoryUpdated() => _=OnInventoryUpdated?.Invoke();
        public static async Task<int> InvokeItemAdded(Item item, int amount) { return await OnItemAdded?.Invoke(item, amount); }
        public static void InvokeItemRemoved(int amount) => _=OnItemRemoved?.Invoke(amount);
        public static void InvokeClearInventory() => _=OnClearInventory?.Invoke();
        public static void InvokeSlotsItemChanged() => OnSlotsItemChanged?.Invoke();
    }
}
