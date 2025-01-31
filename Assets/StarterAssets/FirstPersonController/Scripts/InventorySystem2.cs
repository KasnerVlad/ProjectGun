using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class InventorySystem2 : InventoryBase
{
    public override void InitializeInventory()
    {
        parameters = new InventoryParams(maxSlots, allowStacking);
        slots = new List<InventorySlots>(maxSlots);
        dragAndDrop = GetComponent<DragAndDrop>();
        for (int i = 0; i < maxSlots; i++)
        {
            slots.Add(new InventorySlots());
        }
        for (int i = 0; i < maxSlots; i++)
        {
            slots[i].Slot = i<hotbar.transform.childCount? hotbar.transform.GetChild(i).gameObject : inventory.transform.GetChild(i-hotbar.transform.childCount).gameObject;
        }
        imageArrayChecker = new HashSet<Image>(GetSlotsImage());
        slotsArrayChecker = new HashSet<GameObject>(GetSlotsGameObject());
        
        dragAndDrop.SetCanvas(InventoryCanvas);
        dragAndDrop.SetInventorySlots(slots);

        dragAndDrop.SetInventory(inventory);
        dragAndDrop.SetImageSet(imageArrayChecker);
        dragAndDrop.SetSlotSet(slotsArrayChecker);
        InventoryEvents.InvokeInventoryUpdated(slots);
        Debug.Log("Inventory Updated");
    }
    
    private void Update()
    {
        dragAndDrop.DragAndDropManager(dragspeed);
    }

    public GameObject[] GetSlotsGameObject()
    {
        GameObject[] slotsArray = new GameObject[maxSlots];
        for (int i = 0; i < maxSlots; i++)
        {
            slotsArray[i] = slots[i].Slot;
        }
            
        return slotsArray;
    }
    public Image[] GetSlotsImage()
    {
        Image[] slotsArray = new Image[maxSlots];
        for (int i = 0; i < maxSlots; i++)
        {
            slotsArray[i] = slots[i].Slot.transform.GetChild(0).GetComponent<Image>();
        }
            
        return slotsArray;
    }
    public override void SingOnEvents()
    {
        InventoryEvents.OnItemAdded += AddItem;
        InventoryEvents.OnItemRemoved += RemoveItem;
        InventoryEvents.OnClearInventory += ClearInventory;
    }

    public override void SingOffEvents()
    {
        InventoryEvents.OnItemAdded -= AddItem;
        InventoryEvents.OnItemRemoved -= RemoveItem;
        InventoryEvents.OnClearInventory -= ClearInventory;
    }
    

    public override bool AddItem(Item item, int amount)
    {

        foreach (var slot in slots)
        {
            if (slot.IsEmpty()||(slot.Item == item && parameters.AllowStacking && slot.Amount + amount <= item.maxStackSize))
            {
                slot.AddItem(item, amount);
                InventoryEvents.InvokeInventoryUpdated(slots);
                return true;
            }
        }

        Debug.Log("No space to add item");
        return false;
    }

    public override bool RemoveItem(int amount)
    {
        foreach (var slot in slots)
        {
            if (slot.Amount>=1)
            {
                int removableAmount = Mathf.Min(slot.Amount, amount);
                slot.RemoveItem(removableAmount);
                InventoryEvents.InvokeInventoryUpdated(slots);
                amount -= removableAmount;

                if (amount <= 0) return true;
            }
        }

        Debug.Log("Not enough items to remove");
        return false;
    }

    public override void ClearInventory()
    {
        foreach (var slot in slots)
        {
            slot.ClearSlot();
        }
        InventoryEvents.InvokeInventoryUpdated(slots);
    }
}
