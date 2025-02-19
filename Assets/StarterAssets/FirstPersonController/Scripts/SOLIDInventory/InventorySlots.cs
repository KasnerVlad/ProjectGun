using UnityEngine;
using System.Threading.Tasks;
public class InventorySlots
{
    public Item Item { get; private set; }
    public int Amount { get; private set; }
    public GameObject Slot { get; private set; }

    public async Task AddItem(Item item, int amount)
    {
        await Task.Yield();
        Item = item;
        if(Item!=null&&Amount+amount<=Item.maxStackSize) Amount += amount;
    }

    public async Task RemoveItem(int amount)
    {
        await Task.Yield();
        Amount -= amount;
        if (Amount <= 0) ClearSlot();
    }

    public async Task ClearSlot()
    {
        await Task.Yield();
        Item = null;
        Amount = 0;
    }

    public void SetGSlot(GameObject slot)
    {
        Slot = slot;
    }
    public bool IsEmpty() => Item == null;
}