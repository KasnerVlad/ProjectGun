using UnityEngine;

public class InventorySlots
{
    public Item Item { get; private set; }
    public int Amount { get; private set; }
    public GameObject Slot { get; set; }

    public void AddItem(Item item, int amount)
    {
        Item = item;
        Amount += amount;
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

    public bool IsEmpty() => Item == null;
}