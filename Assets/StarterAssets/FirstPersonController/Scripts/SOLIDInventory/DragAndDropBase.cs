using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Threading.Tasks;

namespace StarterAssets.FirstPersonController.Scripts.SOLIDInventory
{
    public abstract class DragAndDropBase : MonoBehaviour
    {
        protected Canvas Canvas{private set; get;}
        public List<InventorySlots> InventorySlots{private set; get;}
        protected GameObject Inventory{private set; get;}
        protected HashSet<Image> ImageSet{private set; get;}
        protected HashSet<GameObject> SlotSet{private set; get;}
        protected Image DraggedImage{set; get;}
        protected InventorySlots SourceSlot{set; get;}
        protected Vector2 OriginalPosition{set; get;}
        protected bool IsDragging{set; get;}
        
        public void SetCanvas(Canvas targetCanvas) => Canvas = targetCanvas;
        public void SetInventorySlots(List<InventorySlots> slots) => InventorySlots = slots;
        public void SetInventory(GameObject inventoryObj) => Inventory = inventoryObj;
        public void SetImageSet(HashSet<Image> images) => ImageSet = images;
        public void SetSlotSet(HashSet<GameObject> slots) => SlotSet = slots;
        public virtual async Task DragAndDropManager(float dragSpeed)
        {
            if (!IsDragging && InventoryInput.StartDragging)
            {
                await StartDrag();
            }
    
            if (IsDragging)
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
}
