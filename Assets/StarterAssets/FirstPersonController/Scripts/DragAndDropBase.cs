using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Threading.Tasks;
public abstract class DragAndDropBase : MonoBehaviour
{
    protected bool dragging;
    protected RectTransform rectTransform;
    protected Canvas canvas;
    protected CanvasGroup canvasGroup;
    protected GameObject inventory;
    protected HashSet<Image> imageSet;
    protected HashSet<GameObject> slotSet;
    protected int originalParentIndex;
    protected Transform originalParent;
    public List<InventorySlots> inventorySlots;
    public abstract Task StartDragging();
    public abstract Task Dragging(float dragSpeed);
    public abstract Task EndDragging();
    public abstract void DragAndDropManager(float dragSpeed);
    
}