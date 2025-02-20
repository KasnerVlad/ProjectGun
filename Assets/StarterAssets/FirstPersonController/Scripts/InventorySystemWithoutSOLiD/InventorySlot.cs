using System;
using UnityEngine;
using UnityEngine.UI;
using StarterAssets.FirstPersonController.Scripts.SOLIDInventory;
public class InventorySlot
{
    private GameObject slot;
    private Image slotImage;
    private Text slotText;
    private int itemAmount;
    private Item itemInSlot;
    public void ClearSlot()
    {
        itemAmount = 0;
        itemInSlot = null;
        UpdateGui();

        Debug.Log($"inventory slot cleared, amount: {itemAmount}, item: {itemInSlot}, slot Image: {slotImage.gameObject.name}, slot Text: {slotText.name}");
    }
    public bool AddItem(Item item, int amount)
    {
        if (itemAmount + amount <= item.maxStackSize)
        {
            itemInSlot = item;
            itemAmount += amount;
            UpdateGui(); return true;
        }
        else { return false; }
    }
    public bool RemoveItem(int amount)
    {
        itemAmount -= amount;
        if (itemAmount <= 0)
        {
            itemAmount = 0;
            itemInSlot = null;
            UpdateGui(); return false;
        }
        UpdateGui(); return true;
    }
    public void UpdateGui()
    {
        if (slotImage != null)
        {
            slotImage.sprite=itemInSlot==null?null:itemInSlot.icon;
            if (itemAmount <= 0) { slotImage.enabled = false; }
            else { slotImage.enabled = true; }
            
        }
        else { throw new Exception("Can't update image in slot"); }
        if (slotText != null)
        {
            slotText.text = itemAmount.ToString();
            if (itemAmount <= 1) { slotText.text = ""; }
        }
        else { throw new Exception("Can't update text in slot"); }
    }
    public void SetSlotAmount(int amount) { itemAmount = amount; UpdateGui(); }
    public int GetAmount() { return itemAmount; }
    public void SetSlotItem(Item item) { itemInSlot = item; UpdateGui(); }
    public Item GetItem() { return itemInSlot; }
    public void SetInventorySlot(GameObject slot) { this.slot = slot; }
    public GameObject GetInventorySlot() { return slot; }
    public void SetSlotImage(Image image) { slotImage = image; }
    public Image GetSlotImage() { return slotImage; }
    public void SetSlotText(Text text) { slotText = text; }
    public Text GetSlotText() { return slotText; }
}