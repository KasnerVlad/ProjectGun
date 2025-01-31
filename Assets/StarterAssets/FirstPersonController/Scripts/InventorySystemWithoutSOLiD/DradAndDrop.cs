using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DradAndDrop
{
    private bool dragging;
    private RectTransform rectTransform;
    private Canvas canvas;
    private CanvasGroup canvasGroup;
    private GameObject inventory;
    private HashSet<Image> imageArrayChecker;
    private HashSet<GameObject> slotsArrayChecker;
    private int originalParentIndex;
    private Transform originalParent;
    private List<InventorySlot> inventorySlots;
    public void SetCanvas(Canvas canvas) { this.canvas = canvas; }
    public void SetInventory(GameObject inventory) { this.inventory = inventory; }
    public void SetImageArray(HashSet<Image> imageArrayChecker) { this.imageArrayChecker = imageArrayChecker; }
    public void SetSlotArray(HashSet<GameObject> slotsArrayChecker) { this.slotsArrayChecker = slotsArrayChecker; }
    public void SetInventorySlots(List<InventorySlot> inventorySlots) { this.inventorySlots = inventorySlots; }
    
    private void StartDraging()
    {
        if (Input.GetMouseButtonDown(0)&&dragging == false&&inventory.activeSelf)
        {
            
            PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current);
            eventDataCurrentPosition.position = Input.mousePosition;

            List<RaycastResult> results = new List<RaycastResult>();
            GraphicRaycaster gr = canvas.GetComponent<GraphicRaycaster>(); // Получаем GraphicRaycaster с Canvas
            gr.Raycast(eventDataCurrentPosition, results);

            if (results.Count > 0)
            {
                foreach (RaycastResult result in results)
                {
                    if (imageArrayChecker.Contains(result.gameObject.GetComponent<Image>()))
                    {
                        Debug.Log("RectTransform Name: " + result.gameObject.name);
                        Debug.Log("RectTransform Rect: " + result.gameObject.GetComponent<RectTransform>());
                        rectTransform  = result.gameObject.GetComponent<RectTransform>();
                        canvasGroup = result.gameObject.GetComponent<CanvasGroup>();
                        if (slotsArrayChecker.Contains(result.gameObject.transform.parent.gameObject))
                        {
                            originalParent = result.gameObject.transform.parent;
                       
                            for (int i = 0; i < inventorySlots.Count; i++)
                            {
                                if (inventorySlots[i].GetInventorySlot() == result.gameObject.transform.parent.gameObject)
                                {
                                    originalParentIndex = i;
                                    dragging = true;
                                    if (canvasGroup != null && rectTransform != null)
                                    {
                                        canvasGroup.alpha = .6f;
                                        canvasGroup.blocksRaycasts = false;
                                        rectTransform.SetAsLastSibling();
                                    }
                                }
                            }
                        } 
                    }
                }
            }
            else { Debug.Log("Results.Count = 0"); }
        }
    }

    private void Dragging(float dragSpeed)
    {
        if (Input.GetMouseButton(0) && dragging&&inventory.activeSelf)
        {

            Vector2 delta = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));

            rectTransform.anchoredPosition += delta * dragSpeed;
        }
    }

    private void EndDragging()
    {
        if (Input.GetMouseButtonUp(0) && dragging&&inventory.activeSelf)
        {
            dragging = false;
            canvasGroup.alpha = 1f;
            canvasGroup.blocksRaycasts = true;
            PointerEventData eventData;
            eventData = new PointerEventData(EventSystem.current);
            // 2. Устанавливаем позицию указателя
            eventData.position = Input.mousePosition;
            List<RaycastResult> results = new List<RaycastResult>();
            // 3. Выполняем Raycast
            EventSystem.current.RaycastAll(eventData, results);
            Debug.Log(results.Count+" Results Count");
            if (results.Count <= 1&&inventorySlots[originalParentIndex].GetItem().prefab!=null)
            {

                InventorySystem inventorySystem = canvas.gameObject.GetComponent<InventorySystem>();
                GameObject itemPrefab = inventorySystem.InstatePrefab(inventorySlots[originalParentIndex].GetItem().prefab);
                if (itemPrefab != null)
                {
                    if (itemPrefab.GetComponent<ObjectLicens>() != null)
                    {
                        itemPrefab.GetComponent<ObjectLicens>().SetAmount(inventorySlots[originalParentIndex].GetAmount());
                    }
                }
                inventorySlots[originalParentIndex].RemoveItem(inventorySlots[originalParentIndex].GetAmount());
            }
            else if(results.Count <= 1)
            {
                throw new Exception($"Prefab in {inventorySlots[originalParentIndex].GetItem()} not found");
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
                        if (inventorySlots[slotss.IndexOf(g)].GetItem() == inventorySlots[originalParentIndex].GetItem())
                        {
                            if (inventorySlots[slotss.IndexOf(g)].GetAmount() == inventorySlots[slotss.IndexOf(g)].GetItem().maxStackSize)
                            {
                                SwapItems( slotss.IndexOf(g));
                                Debug.Log("Swapped Item");
                                ReturnToOriginalSlot();
                                return;
                            }
                            int remainingSpace = inventorySlots[slotss.IndexOf(g)].GetItem().maxStackSize - inventorySlots[slotss.IndexOf(g)].GetAmount();
                            int amountToMove = Mathf.Min(inventorySlots[originalParentIndex].GetAmount(), remainingSpace);
                            int originalSlotNewAmount = inventorySlots[originalParentIndex].GetAmount() - amountToMove;
                            MoveItems(slotss.IndexOf(g), amountToMove, originalSlotNewAmount);
                            ReturnToOriginalSlot();
                            return;
                        }
                        else if (inventorySlots[slotss.IndexOf(g)].GetItem() == null)
                        {
                            MoveAllItems(slotss.IndexOf(g));
                            ReturnToOriginalSlot();
                            return;
                        }
                        else if (inventorySlots[slotss.IndexOf(g)].GetItem()!= null &&
                                 inventorySlots[slotss.IndexOf(g)].GetItem() != inventorySlots[originalParentIndex].GetItem())
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
    }
    public void DragAndDropManager(float dragSpeed)
    {
        StartDraging();
        Dragging(dragSpeed);
        EndDragging();
    }

    private void MoveAllItems(int targetSlot)
    {
        Item originalItem = inventorySlots[originalParentIndex].GetItem();
        int originalAmount = inventorySlots[originalParentIndex].GetAmount();

        inventorySlots[targetSlot].AddItem(originalItem, originalAmount);
        inventorySlots[originalParentIndex].RemoveItem(originalAmount);
        UpdateGuiInArray();
    }
    private void MoveItems(int targetSlot, int amountToMove, int originalSlotNewAmount)
    {

        Item originalItem = inventorySlots[originalParentIndex].GetItem();
        Item targetItem = inventorySlots[targetSlot].GetItem();
        int originalAmount = inventorySlots[originalParentIndex].GetAmount();
        inventorySlots[targetSlot].AddItem(targetItem, amountToMove);
        inventorySlots[originalParentIndex].RemoveItem(originalAmount);
        inventorySlots[originalParentIndex].AddItem(originalItem, originalSlotNewAmount);
        UpdateGuiInArray();
    }
    private void SwapItems(int targetSlot)
    {
        Item targetItem = inventorySlots[targetSlot].GetItem();
        int targetAmount = inventorySlots[targetSlot].GetAmount();
        
        Item originalItem = inventorySlots[originalParentIndex].GetItem();
        int originalAmount = inventorySlots[originalParentIndex].GetAmount();
        
        inventorySlots[targetSlot].RemoveItem(inventorySlots[targetSlot].GetAmount());
        inventorySlots[originalParentIndex].RemoveItem(inventorySlots[originalParentIndex].GetAmount());
        
        inventorySlots[originalParentIndex].AddItem(targetItem, targetAmount);
        inventorySlots[targetSlot].AddItem(originalItem, originalAmount);
        
        UpdateGuiInArray();
        
        
    }
    private void ReturnToOriginalSlot()
    {
        if (inventorySlots[originalParentIndex].GetInventorySlot() != null)
        {
            rectTransform.transform.SetParent(originalParent);
            rectTransform.transform.localPosition = Vector3.zero;
        }
    }

    private void UpdateGuiInArray() { for (int i = 0; i < inventorySlots.Count; i++) { inventorySlots[i].UpdateGui(); } }

    private GameObject[] GetInventorySlotsArray()
    {
        GameObject[] slotsArray = new GameObject[inventorySlots.Count];
        for (int i = 0; i < inventorySlots.Count; i++)
        {
            slotsArray[i] = inventorySlots[i].GetInventorySlot();
        }
        return slotsArray;
    }
}