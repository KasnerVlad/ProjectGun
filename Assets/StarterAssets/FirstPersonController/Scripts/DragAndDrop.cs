using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Threading.Tasks;
public class DragAndDrop : DragAndDropBase
{

    public void SetCanvas(Canvas canvas) => this.canvas = canvas;
    public void SetInventory(GameObject inventory) => this.inventory = inventory;
    public void SetImageSet(HashSet<Image> imageSet) => this.imageSet = imageSet;
    public void SetSlotSet(HashSet<GameObject> slotSet) => this.slotSet = slotSet;
    public void SetInventorySlots(List<InventorySlots> inventorySlots) => this.inventorySlots = inventorySlots;

    public override async Task StartDragging()
    {
        await Task.Yield();
        if (!InventoryInput.StartDragging || dragging || !inventory.activeSelf) return;
        
        PointerEventData eventData = new PointerEventData(EventSystem.current) { position = Input.mousePosition };
        List<RaycastResult> results = new List<RaycastResult>();
        canvas.GetComponent<GraphicRaycaster>().Raycast(eventData, results);

        foreach (RaycastResult result in results)
        {
            if (!imageSet.Contains(result.gameObject.GetComponent<Image>())) continue;
            
            rectTransform = result.gameObject.GetComponent<RectTransform>();
            canvasGroup = result.gameObject.GetComponent<CanvasGroup>();
            originalParent = result.gameObject.transform.parent;

            for (int i = 0; i < inventorySlots.Count; i++)
            {
                if (inventorySlots[i].Slot == originalParent.gameObject)
                {
                    originalParentIndex = i;
                    dragging = true;
                    canvasGroup.alpha = 0.6f;
                    canvasGroup.blocksRaycasts = false;
                    rectTransform.SetAsLastSibling();
                    return;
                }
            }
        }
    }

    public override async Task Dragging(float dragSpeed)
    {
        await Task.Yield();
        if (!dragging || !InventoryInput.Dragging || !inventory.activeSelf) return;
        rectTransform.anchoredPosition += new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y")) * dragSpeed;
    }

    public override async Task EndDragging()
    {
        await Task.Yield();
        if (!InventoryInput.EndDragging || !dragging || !inventory.activeSelf) return;
        
        dragging = false;
        canvasGroup.alpha = 1f;
        canvasGroup.blocksRaycasts = true;
        
        PointerEventData eventData = new PointerEventData(EventSystem.current) { position = Input.mousePosition };
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, results);

        if (results.Count <= 1&& inventorySlots[originalParentIndex].Item.prefab!=null)
        {
            GameObject itemPrefab = Instantiate(inventorySlots[originalParentIndex].Item.prefab, Vector3.zero, Quaternion.identity);
            if (itemPrefab?.GetComponent<ObjectLicens>() != null)
            {
                itemPrefab.GetComponent<ObjectLicens>().SetAmount(inventorySlots[originalParentIndex].Amount);
            }
            else
            {
                Destroy(itemPrefab);
            }
            
            inventorySlots[originalParentIndex].RemoveItem(inventorySlots[originalParentIndex].Amount);
            ReturnToOriginalSlot();
            return;
        }
        
        foreach (RaycastResult result in results)
        {
            List<GameObject> slotss = new List<GameObject>(GetInventorySlotsArray());
            foreach (GameObject g in slotss)
            {
                if (g == result.gameObject)
                {
                    GameObject targetSlot  = result.gameObject;
                    if (targetSlot == slotss[originalParentIndex])
                    {
                        ReturnToOriginalSlot();
                        return;
                            
                    }
                    if (inventorySlots[slotss.IndexOf(g)].Item == inventorySlots[originalParentIndex].Item)
                    {
                        if (inventorySlots[slotss.IndexOf(g)].Amount == inventorySlots[slotss.IndexOf(g)].Item.maxStackSize)
                        {
                            SwapItems( slotss.IndexOf(g));
                            Debug.Log("Swapped Item");
                            ReturnToOriginalSlot();
                            return;
                        }
                        int remainingSpace = inventorySlots[slotss.IndexOf(g)].Item.maxStackSize - inventorySlots[slotss.IndexOf(g)].Amount;
                        int amountToMove = Mathf.Min(inventorySlots[originalParentIndex].Amount, remainingSpace);
                        int originalSlotNewAmount = inventorySlots[originalParentIndex].Amount - amountToMove;
                        MoveItems(slotss.IndexOf(g), amountToMove, originalSlotNewAmount);
                        ReturnToOriginalSlot();
                        return;
                    }
                    else if (inventorySlots[slotss.IndexOf(g)].Item == null)
                    {
                        MoveAllItems(slotss.IndexOf(g));
                        ReturnToOriginalSlot();
                        return;
                    }
                    else if (inventorySlots[slotss.IndexOf(g)].Item!= null &&
                             inventorySlots[slotss.IndexOf(g)].Item != inventorySlots[originalParentIndex].Item)
                    {
                        SwapItems(slotss.IndexOf(g));
                        ReturnToOriginalSlot();
                        return;
                    }
                }
            }
        }

        ReturnToOriginalSlot();
    }

    private List<GameObject> GetInventorySlotsArray()
    {
        List<GameObject> slots = new List<GameObject>();
        for (int i = 0; i < inventorySlots.Count; i++)
        {
            slots.Add(inventorySlots[i].Slot);
        }
        return slots;
    }

    private void MoveAllItems(int targetSlot)
    {
        Item originalItem = inventorySlots[originalParentIndex].Item;
        int originalAmount = inventorySlots[originalParentIndex].Amount;

        inventorySlots[targetSlot].AddItem(originalItem, originalAmount);
        inventorySlots[originalParentIndex].RemoveItem(originalAmount);
        InventoryEvents.InvokeInventoryUpdated(inventorySlots);
    }
    private void MoveItems(int targetSlot, int amountToMove, int originalSlotNewAmount)
    {

        Item originalItem = inventorySlots[originalParentIndex].Item;
        Item targetItem = inventorySlots[targetSlot].Item;
        int originalAmount = inventorySlots[originalParentIndex].Amount;
        inventorySlots[targetSlot].AddItem(targetItem, amountToMove);
        inventorySlots[originalParentIndex].RemoveItem(originalAmount);
        inventorySlots[originalParentIndex].AddItem(originalItem, originalSlotNewAmount);
        InventoryEvents.InvokeInventoryUpdated(inventorySlots);
    }
    private void SwapItems(int targetSlot)
    {
        Item targetItem = inventorySlots[targetSlot].Item;
        int targetAmount = inventorySlots[targetSlot].Amount;
        
        Item originalItem = inventorySlots[originalParentIndex].Item;
        int originalAmount = inventorySlots[originalParentIndex].Amount;
        
        inventorySlots[targetSlot].RemoveItem(inventorySlots[targetSlot].Amount);
        inventorySlots[originalParentIndex].RemoveItem(inventorySlots[originalParentIndex].Amount);
        
        inventorySlots[originalParentIndex].AddItem(targetItem, targetAmount);
        inventorySlots[targetSlot].AddItem(originalItem, originalAmount);
        
        InventoryEvents.InvokeInventoryUpdated(inventorySlots);
        
        
    }
    private void ReturnToOriginalSlot()
    {
        rectTransform.SetParent(originalParent);
        rectTransform.localPosition = Vector3.zero;
    }
    
    
    public override void DragAndDropManager(float dragSpeed)
    {
        StartDragging();
        Dragging(dragSpeed);
        EndDragging();
    }
    
    
}