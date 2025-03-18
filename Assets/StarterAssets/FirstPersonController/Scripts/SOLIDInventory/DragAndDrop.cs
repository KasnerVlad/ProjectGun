using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Threading.Tasks;

namespace StarterAssets.FirstPersonController.Scripts.SOLIDInventory
{
    public class DragAndDrop : DragAndDropBase
    {
        protected override async Task StartDrag()
        {
            GameObject clickedObject = GetClickedSlot();
            if (clickedObject != null)
            {
                int slotIndex = InventorySlots.FindIndex(s => s.Slot == clickedObject);
                SourceSlot = InventorySlots[slotIndex];
                
                if (!SourceSlot.IsEmpty())
                {
                    DraggedImage = SourceSlot.Slot.transform.GetChild(0).GetComponent<Image>();
                    OriginalPosition = DraggedImage.rectTransform.position;
                    IsDragging = true;
                }
            }
            await Task.Yield();
        }
    
        protected override void Dragging(float speed)
        {
            if (DraggedImage != null && Inventory.activeSelf&&SourceSlot.Item != null)
            {
                RectTransform rectTransform = DraggedImage.rectTransform;
                float mouseX = UnityEngine.Input.GetAxis("Mouse X") * speed * 10;
                float mouseY = UnityEngine.Input.GetAxis("Mouse Y") * speed * 10;
                rectTransform.anchoredPosition += new Vector2(mouseX, mouseY);
            }
            else { DraggedImage.rectTransform.localPosition = Vector3.zero; IsDragging = false; }
        }
    
        protected override async Task EndDrag()
        {
            GameObject targetObject = GetHoveredSlot();
            if (targetObject != null && SlotSet.Contains(targetObject))
            {
                int targetIndex = InventorySlots.FindIndex(s => s.Slot == targetObject);
                InventorySlots targetSlot = InventorySlots[targetIndex];
                if (targetSlot.Item != SourceSlot.Item || (targetSlot.Amount == targetSlot.Item.maxStackSize))
                {
                    SwapItems(SourceSlot, targetSlot);
                }
                else if (targetSlot.Amount < targetSlot.Item.maxStackSize&&targetSlot.Item==SourceSlot.Item)
                {
                    MoveItems(SourceSlot, targetSlot);
                }

            }
            DraggedImage.rectTransform.localPosition = Vector3.zero;
            IsDragging = false;
            InventoryEvents.InvokeInventoryUpdated();
            await Task.Yield();
        }
        
        private void SwapItems(InventorySlots from, InventorySlots to)
        {
            if (from == to) return;
    
            Item tempItem = from.Item;
            int tempAmount = from.Amount;
            
            Item newItem = to.Item;
            int newAmount = to.Amount;
            
            from.ClearSlot();
            to.ClearSlot();

            to.AddItem(tempItem, tempAmount);
            from.AddItem(newItem, newAmount);
        }
    
        private void MoveItems(InventorySlots from, InventorySlots to)
        {
            if (from == to) return;
            int remainingSpace = to.Item.maxStackSize - to.Amount;
            int amountToMove = Math.Min(remainingSpace, from.Amount);
            int sourceNewAmount = from.Amount-amountToMove;
            Item originalItem = from.Item;
            Item targetItem = to.Item;
            int originalAmount = from.Amount;
    
            from.RemoveItem(originalAmount);
            to.AddItem(targetItem, amountToMove);
            from.AddItem(originalItem, sourceNewAmount);
        }
        private GameObject GetClickedSlot()
        {
            if (InventoryInput.StartDragging)
            {
                Vector2 mousePos = UnityEngine.Input.mousePosition;
                GraphicRaycaster raycaster = Canvas.GetComponent<GraphicRaycaster>();
                
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
                    if (SlotSet.Contains(result.gameObject))
                    {
                        return result.gameObject;
                    }
                }
            }
            return null;
        }
    
        private GameObject GetHoveredSlot()
        {
            Vector2 mousePos = UnityEngine.Input.mousePosition;
            GraphicRaycaster raycaster = Canvas.GetComponent<GraphicRaycaster>();
            
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
                if (SlotSet.Contains(result.gameObject))
                {
                    return result.gameObject;
                }
            }
            return null;
        }
        
    }
}
