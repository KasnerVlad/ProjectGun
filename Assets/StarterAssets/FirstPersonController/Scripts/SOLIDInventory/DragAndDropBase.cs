using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Threading.Tasks;
public abstract class DragAndDropBase : MonoBehaviour
{
    protected Canvas canvas{private set; get;}
    public List<InventorySlots> inventorySlots{private set; get;}
    protected GameObject inventory{private set; get;}
    protected HashSet<Image> imageSet{private set; get;}
    protected HashSet<GameObject> slotSet{private set; get;}
    protected Image draggedImage{set; get;}
    protected InventorySlots sourceSlot{set; get;}
    protected Vector2 originalPosition{set; get;}
    protected bool isDragging{set; get;}
    
    public void SetCanvas(Canvas targetCanvas) => canvas = targetCanvas;
    public void SetInventorySlots(List<InventorySlots> slots) => inventorySlots = slots;
    public void SetInventory(GameObject inventoryObj) => inventory = inventoryObj;
    public void SetImageSet(HashSet<Image> images) => imageSet = images;
    public void SetSlotSet(HashSet<GameObject> slots) => slotSet = slots;
    public virtual async void DragAndDropManager(float dragSpeed)
    {
        if (!isDragging && InventoryInput.StartDragging)
        {
            await StartDrag();
        }

        if (isDragging)
        {
            Dragging(dragSpeed);
            
            if (InventoryInput.EndDragging)
            {
                await EndDrag();
            }
        }
    }
    protected abstract Task StartDrag();
    protected abstract Task EndDrag();
    protected abstract void Dragging(float dragSpeed);
    
}