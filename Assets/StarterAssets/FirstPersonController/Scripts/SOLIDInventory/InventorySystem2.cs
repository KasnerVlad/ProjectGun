using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Threading.Tasks;
namespace StarterAssets.FirstPersonController.Scripts.SOLIDInventory
{
    public class InventorySystem2 : InventoryBase
    {
        private void OnDisable() => SaveManager._GameSaveManager.SaveSlots();
        protected override void InitializeInventory()
        {
            parameters = new InventoryParams(maxSlots, allowStacking);
            dragAndDrop = GetComponent<DragAndDrop>();
            for (int i = 0; i < maxSlots; i++)
            {
                slots.Add(new InventorySlots());
            }
            for (int i = 0; i < maxSlots; i++)
            {
                slots[i].SetGSlot(i<hotbar.transform.childCount? hotbar.transform.GetChild(i).gameObject : inventory.transform.GetChild(i-hotbar.transform.childCount).gameObject);
            }
            imageArrayChecker = new HashSet<Image>(GetSlotsImage());
            slotsArrayChecker = new HashSet<GameObject>(GetSlotsGameObject());
            
            dragAndDrop.SetCanvas(InventoryCanvas);
            dragAndDrop.SetInventorySlots(slots);
    
            dragAndDrop.SetInventory(inventory);
            dragAndDrop.SetImageSet(imageArrayChecker);
            dragAndDrop.SetSlotSet(slotsArrayChecker);
            
            SaveManager saveManager = GetComponent<SaveManager>();
            HotBarManager hbManager = GetComponent<HotBarManager>();

            saveManager.InitializeSlots(slots);
            hbManager.Initialize(slots);
        }
        public List<Item> GetSlotsItems()
        {
            List<Item> items = new List<Item>(maxSlots);
            for (int i = 0; i < maxSlots; i++)
            {
                items.Add(slots[i].Item);
            }
            return items;
        }
        public HashSet<GameObject> GetSlotSet() { return slotsArrayChecker; }
        public List<int> GetSlotsItemAmount()
        {
            List<int> itemAmount = new List<int>(maxSlots);
            for (int i = 0; i < maxSlots; i++)
            {
                itemAmount.Add(slots[i].Amount);
            }
            return itemAmount;
        }
        private void Update()=>dragAndDrop.DragAndDropManager(dragspeed);
        private GameObject[] GetSlotsGameObject()
        {
            GameObject[] slotsArray = new GameObject[maxSlots];
            for (int i = 0; i < maxSlots; i++)
            {
                slotsArray[i] = slots[i].Slot;
            }
            return slotsArray;
        }
        private Image[] GetSlotsImage()
        {
            Image[] slotsArray = new Image[maxSlots];
            for (int i = 0; i < maxSlots; i++)
            {
                slotsArray[i] = slots[i].Slot.transform.GetChild(0).GetComponent<Image>();
            }
                
            return slotsArray;
        }
        protected override void SingOnEvents()
        {
            InventoryEvents.OnItemAdded += AddItem;
            InventoryEvents.OnItemRemoved += RemoveItem;
            InventoryEvents.OnClearInventory += ClearInventory;
        }
    
        protected override void SingOffEvents()
        {
            InventoryEvents.OnItemAdded -= AddItem;
            InventoryEvents.OnItemRemoved -= RemoveItem;
            InventoryEvents.OnClearInventory -= ClearInventory;
        }
        public override async Task<int> AddItem(Item item, int amount)
        {
            await Task.Yield();
            int remainingAmount = amount;
    
            // Сначала пытаемся добавить в существующие стеки
            if (parameters.AllowStacking)
            {
                foreach (var slot in slots)
                {
                    if (slot.Item == item && slot.Amount < item.maxStackSize)
                    {
                        int availableSpace = item.maxStackSize - slot.Amount;
                        int addAmount = Mathf.Min(availableSpace, remainingAmount);
                    
                        slot.AddItem(item, addAmount);
                        remainingAmount -= addAmount;
                        if (remainingAmount <= 0)
                        {
                            InventoryEvents.InvokeSlotsItemChanged();
                            InventoryEvents.InvokeInventoryUpdated();
                            return 0;
                        }
                    }
                }
            }
    
            // Затем пытаемся добавить в пустые слоты
            foreach (var slot in slots)
            {
                if (slot.IsEmpty())
                {
                    int addAmount = Mathf.Min(item.maxStackSize, remainingAmount);
                
                    slot.AddItem(item, addAmount);
                    remainingAmount -= addAmount;
                    if (remainingAmount <= 0)
                    {
                        InventoryEvents.InvokeSlotsItemChanged();
                        InventoryEvents.InvokeInventoryUpdated();
                        return 0;
                    }
                }
            }
            InventoryEvents.InvokeSlotsItemChanged();
            InventoryEvents.InvokeInventoryUpdated();
            Debug.Log("No space to add item");
            return remainingAmount;
        }
    
        public override async Task<bool> RemoveItem(int amount)
        {
            int remaining = amount;
            foreach (var slot in slots)
            {
                if (slot.Amount >= 1)
                {
                    int removable = Mathf.Min(slot.Amount, remaining);
                    slot.RemoveItem(removable);
                    remaining -= removable;
                    if (remaining <= 0)
                        break;
                    
                }
            }
            if (remaining <= 0)
            {
                InventoryEvents.InvokeSlotsItemChanged();
                InventoryEvents.InvokeInventoryUpdated();
                await Task.Yield();
                return true;
            }
            InventoryEvents.InvokeSlotsItemChanged();
            InventoryEvents.InvokeInventoryUpdated();
            await Task.Yield();
            return false;

        }
        public override async Task ClearInventory()
        {
            foreach (var slot in slots)
            {
                slot.ClearSlot();
            }
            InventoryEvents.InvokeSlotsItemChanged();
            InventoryEvents.InvokeInventoryUpdated();
            await Task.Yield();
        }
    }
}
