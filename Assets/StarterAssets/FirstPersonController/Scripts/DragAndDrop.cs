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
            await SwapItems(sourceSlot, targetSlot);

            InventoryEvents.InvokeInventoryUpdated(inventorySlots);
        }
        draggedImage.rectTransform.position = originalPosition;
        isDragging = false;
        await Task.Yield();
    }
    
    private async Task SwapItems(InventorySlots from, InventorySlots to)
    {
        if (from == to) return;

        Item tempItem = from.Item;
        int tempAmount = from.Amount;

        await from.ClearSlot();
        if (!to.IsEmpty())
        {
            await from.AddItem(to.Item, to.Amount);
            await to.ClearSlot();
        }
        await to.AddItem(tempItem, tempAmount);
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