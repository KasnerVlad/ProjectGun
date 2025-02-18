using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Threading.Tasks;
public class DragAndDrop : DragAndDropBase
{
    protected override async Task StartDrag()
    {
        GameObject clickedObject = GetClickedSlot();
        if (clickedObject != null)
        {
            int slotIndex = inventorySlots.FindIndex(s => s.Slot == clickedObject);
            sourceSlot = inventorySlots[slotIndex];
            
            if (!sourceSlot.IsEmpty())
            {
                draggedImage = sourceSlot.Slot.transform.GetChild(0).GetComponent<Image>();
                originalPosition = draggedImage.rectTransform.position;
                isDragging = true;
            }
        }
        await Task.Yield();
    }

    protected override void Dragging(float speed)
    {
        if (draggedImage != null && inventory.activeSelf)
        {
            RectTransform rectTransform = draggedImage.rectTransform;
            float mouseX = Input.GetAxis("Mouse X") * speed * 10;
            float mouseY = Input.GetAxis("Mouse Y") * speed * 10;
            rectTransform.anchoredPosition += new Vector2(mouseX, mouseY);
        }
    }

    protected override async Task EndDrag()
    {
        GameObject targetObject = GetHoveredSlot();
        if (targetObject != null && slotSet.Contains(targetObject))
        {
            int targetIndex = inventorySlots.FindIndex(s => s.Slot == targetObject);
            InventorySlots targetSlot = inventorySlots[targetIndex];
            if (targetSlot.Item != sourceSlot.Item || (targetSlot.Amount == targetSlot.Item.maxStackSize))
            {
                await SwapItems(sourceSlot, targetSlot);
            }
            else if (targetSlot.Amount < targetSlot.Item.maxStackSize&&targetSlot.Item==sourceSlot.Item)
            {
                await MoveItems(sourceSlot, targetSlot);
            }
            InventoryEvents.InvokeInventoryUpdated(inventorySlots);
        }
        draggedImage.rectTransform.localPosition = Vector3.zero;
        isDragging = false;
        await Task.Yield();
    }
    
    private async Task SwapItems(InventorySlots from, InventorySlots to)
    {
        if (from == to) return;

        Item tempItem = from.Item;
        int tempAmount = from.Amount;
        
        Item newItem = to.Item;
        int newAmount = to.Amount;
        
        await from.ClearSlot();
        await to.ClearSlot();
        
        await to.AddItem(tempItem, tempAmount);
        await from.AddItem(newItem, newAmount);
    }

    private async Task MoveItems(InventorySlots from, InventorySlots to)
    {
        if (from == to) return;
        int remainingSpace = to.Item.maxStackSize - to.Amount;
        int amountToMove = Math.Min(remainingSpace, from.Amount);
        int sourceNewAmount = from.Amount-amountToMove;
        Item originalItem = from.Item;
        Item targetItem = to.Item;
        int originalAmount = from.Amount;

        await from.RemoveItem(originalAmount);
        await to.AddItem(targetItem, amountToMove);
        await from.AddItem(originalItem, sourceNewAmount);
    }
    private GameObject GetClickedSlot()
    {
        if (InventoryInput.StartDragging)
        {
            Vector2 mousePos = Input.mousePosition;
            GraphicRaycaster raycaster = canvas.GetComponent<GraphicRaycaster>();
            
            var pointerEventData = new UnityEngine.EventSystems.PointerEventData(
                UnityEngine.EventSystems.EventSystem.current
            )
            {
                position = mousePos
            };

            List<UnityEngine.EventSystems.RaycastResult> results = new List<UnityEngine.EventSystems.RaycastResult>();
            raycaster.Raycast(pointerEventData, results);

            foreach (var result in results)
            {
                if (slotSet.Contains(result.gameObject))
                {
                    return result.gameObject;
                }
            }
        }
        return null;
    }

    private GameObject GetHoveredSlot()
    {
        Vector2 mousePos = Input.mousePosition;
        GraphicRaycaster raycaster = canvas.GetComponent<GraphicRaycaster>();
        
        var pointerEventData = new UnityEngine.EventSystems.PointerEventData(
            UnityEngine.EventSystems.EventSystem.current
        )
        {
            position = mousePos
        };

        List<UnityEngine.EventSystems.RaycastResult> results = new List<UnityEngine.EventSystems.RaycastResult>();
        raycaster.Raycast(pointerEventData, results);

        foreach (var result in results)
        {
            if (slotSet.Contains(result.gameObject))
            {
                return result.gameObject;
            }
        }
        return null;
    }
    
}