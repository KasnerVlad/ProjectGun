using System.Collections.Generic;

public static class InventoryEvents
{
    public delegate void InventoryUpdatedHandler(List<InventorySlots> slots);
    public static event InventoryUpdatedHandler OnInventoryUpdated;

    public delegate bool ItemAddedHandler(Item item, int amount);
    public static event ItemAddedHandler OnItemAdded;

    public delegate bool ItemRemovedHandler(int amount);
    public static event ItemRemovedHandler OnItemRemoved;
    
    public delegate void ClearInventory();
    public static event ClearInventory OnClearInventory;
    public static void InvokeInventoryUpdated(List<InventorySlots> inventorySlotsList) => OnInventoryUpdated?.Invoke(inventorySlotsList);
    public static void InvokeItemAdded(Item item, int amount) => OnItemAdded?.Invoke(item, amount);
    public static void InvokeItemRemoved(int amount) => OnItemRemoved?.Invoke(amount);
    public static void InvokeClearInventory() => OnClearInventory?.Invoke();
}