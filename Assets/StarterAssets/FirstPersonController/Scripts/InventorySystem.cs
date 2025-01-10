using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEditor.Timeline.Actions;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;
using Image = UnityEngine.UI.Image;
using UnityEngine.EventSystems;
public class InventorySystem : MonoBehaviour
{
    [Header("Inventory")]
    private GameObject[] slots = new GameObject[32];
    private GameObject[] slotsImages = new GameObject[32];
    private GameObject[] slotsTexts = new GameObject[32];
    private int[] itemAmount = new int[32];
    private Item[] itemInSlots = new Item[32];
    [SerializeField] private GameObject inventory;
    [SerializeField] private GameObject hotBar;

    public delegate void AddItemToInventoryDelegate(int itemAmount, Item item);
    
    public delegate void RemoveItemFromInventoryDelegate(int itemAmount, Item item);
    
    public delegate void ClearInventoryDelegate();
    
    public delegate void RemoveItemFromInventory(int itemAmount);

    public delegate void DragAndDropManagerDelegat();
    
    public delegate void InventoryManagerDelegat();
    
    public static AddItemToInventoryDelegate OnAddItemToInventoryDelegate;
    public static RemoveItemFromInventoryDelegate OnRemoveItemFromInventoryDelegate;
    public static ClearInventoryDelegate OnClearInventoryDelegate;
    public static RemoveItemFromInventory OnRemoveItemFromInventory;
    
    public static DragAndDropManagerDelegat OnDragAndDropManagerDelegat;
    
    public static InventoryManagerDelegat OnInventoryManagerDelegat;
    
    private int currentSlot;

    [Header("Drag and Drop")]
    public bool dragging;
    private RectTransform rectTransform;
    private Transform originalParent;
    private int originalParentIndex;
    private HashSet<GameObject> imageArrayChecker;
    private HashSet<GameObject> slotsArrayChecker;
    
    public Canvas canvas;
    private CanvasGroup canvasGroup;
    public float dragSpeed = 1f;
    
    [Header("TestInventory")]
    [SerializeField] private Item testItem1;
    [SerializeField] private Item testItem2;
 // Объявление как поле класса
    private void Awake()
    {
        OnAddItemToInventoryDelegate = AddItemsToInventoryM;
        OnRemoveItemFromInventoryDelegate = RemoveItemsFromInventoryMD;
        OnClearInventoryDelegate = ClearInventoryM;
        OnRemoveItemFromInventory = RemoveItemsFromInventoryM;
        OnDragAndDropManagerDelegat = DragAndDropManager;
        OnInventoryManagerDelegat = InventoryManager;
        for (int i = 0; i < itemAmount.Length; i++)
        {
            itemAmount[i] = 0;
        }
        for (int i = 0; i < itemInSlots.Length; i++)
        {
            itemInSlots[i] = null;
        }

        for (int i = 0; i < slots.Length; i++)
        {
            if (i < hotBar.transform.childCount)
            {
                slots[i] = hotBar.transform.GetChild(i).gameObject;
          
            }
            else if(i-hotBar.transform.childCount < inventory.transform.childCount)
            {
                slots[i] = inventory.transform.GetChild(i-hotBar.transform.childCount).gameObject;
               
            }
        }

        for (int i = 0; i < slots.Length; i++)
        {
            if (slots[i] != null)
            {
                slotsImages[i] = slots[i].transform.GetChild(0).gameObject;
                slotsTexts[i] = slots[i].transform.GetChild(0).gameObject.transform.GetChild(0).gameObject;
                /*Debug.Log($"Image: {slots[i].transform.GetChild(0).gameObject}, index: {i}");
                Debug.Log($"Text: {slots[i].transform.GetChild(0).gameObject.transform.GetChild(0).gameObject}, index: {i}");*/
            }
            else
            {
                Debug.Log($"Inventory slots not found, index{i}");
            }

        }
        imageArrayChecker = new HashSet<GameObject>(slotsImages);
        slotsArrayChecker = new HashSet<GameObject>(slots);
    }

    void Start()
    {
        for (int i = 0; i < slotsImages.Length; i++)
        {
            if (slotsImages[i] != null)
            {
                slotsImages[i].GetComponent<Image>().enabled = false;
            }
            else
            {
                Debug.Log($"Image: {slotsImages[i].transform.GetChild(0).gameObject}, index: {i} is null");
            }
        }

    }

    // Update is called once per frame
    void Update()
    {
        InventorySystem.OnInventoryManagerDelegat?.Invoke();
        InventorySystem.OnDragAndDropManagerDelegat?.Invoke();
    }

    private void DragAndDropManager()
    {
        
        if (Input.GetMouseButtonDown(0)&&dragging == false)
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
                    if (imageArrayChecker.Contains(result.gameObject))
                    {
                        Debug.Log("RectTransform Name: " + result.gameObject.name);
                        Debug.Log("RectTransform Rect: " + result.gameObject.GetComponent<RectTransform>());
                        rectTransform  = result.gameObject.GetComponent<RectTransform>();
                        canvasGroup = result.gameObject.GetComponent<CanvasGroup>();
                        if (slotsArrayChecker.Contains(result.gameObject.transform.parent.gameObject))
                        {
                            originalParent = result.gameObject.transform.parent;
                       
                            for (int i = 0; i < slots.Length; i++)
                            {
                                if (slots[i] == result.gameObject.transform.parent.gameObject)
                                {
                                    originalParentIndex = i;
                                    dragging = true;
                                }
                            }
                                               
                        } 
                    }


                }
            }
            
            if (canvasGroup != null && rectTransform != null)
            {
                canvasGroup.alpha = .6f;
                canvasGroup.blocksRaycasts = false;
                rectTransform.SetAsLastSibling();
            }
        }

        if (Input.GetMouseButton(0) && dragging)
        {

            Vector2 delta = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));

            rectTransform.anchoredPosition += delta * dragSpeed;
        }

        if (Input.GetMouseButtonUp(0) && dragging)
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
            foreach (RaycastResult result in results)
            {
                List<GameObject> slotss = new List<GameObject>(slots);
                foreach (GameObject g in slots)
                {
                    if (g == result.gameObject)
                    {
                        GameObject targetSlot  = result.gameObject;
                        if (targetSlot == slots[originalParentIndex])
                        {
                            ReturnToOriginalSlot(rectTransform);
                            return;
                            
                        }

                        if (itemInSlots[slotss.IndexOf(g)] == itemInSlots[originalParentIndex])
                        {
                            if (itemAmount[slotss.IndexOf(g)] == itemInSlots[slotss.IndexOf(g)].maxStackSize)
                            {
                                SwapItems(originalParentIndex, slotss.IndexOf(g));
                                Debug.Log("Swapped Item");
                                ReturnToOriginalSlot(rectTransform);
                                return;
                            }
                            int remainingSpace = itemInSlots[slotss.IndexOf(g)].maxStackSize - itemAmount[slotss.IndexOf(g)];
                            int amountToMove = Mathf.Min(itemAmount[originalParentIndex], remainingSpace);
                            int originalSlotNewAmount = itemAmount[originalParentIndex] - amountToMove;
                            MoveItems(slotss.IndexOf(g), amountToMove, originalSlotNewAmount, originalParentIndex);
                            ReturnToOriginalSlot(rectTransform);
                            return;
                        }
                        else if (itemInSlots[slotss.IndexOf(g)] == null)
                        {
                            MoveAllItems(slotss.IndexOf(g), originalParentIndex);
                            ReturnToOriginalSlot(rectTransform);
                            return;
                        }
                        else if (itemInSlots[slotss.IndexOf(g)]!= null && itemInSlots[slotss.IndexOf(g)] != itemInSlots[originalParentIndex])
                        {
                            SwapItems(originalParentIndex, slotss.IndexOf(g));
                            ReturnToOriginalSlot(rectTransform);
                            return;
                        }
                    }
                }

            }

            ReturnToOriginalSlot(rectTransform);
        }
    }

    private void MoveAllItems(int targetSlot, int originalSlot)
    {
        Item originalItem = itemInSlots[originalSlot];
        int originalAmount = itemAmount[originalSlot];
        AddItemToSlot(originalAmount, originalItem, targetSlot);
        RemoveItemFromSlot(originalAmount, originalSlot);
        UpdateGui();
    }
    private void MoveItems(int targetSlot, int amountToMove, int originalSlotNewAmount, int originalSlot)
    {


        AddItemToSlot(amountToMove,itemInSlots[targetSlot], targetSlot);
        RemoveItemFromSlot(itemAmount[originalSlot], originalSlot);
        AddItemToSlot(originalSlotNewAmount, itemInSlots[originalSlot], originalSlot);

        UpdateGui();
        

    }
    private void SwapItems(int originalSlot, int targetSlot)
    {
        Item targetItem = itemInSlots[targetSlot];
        int targetAmount = itemAmount[targetSlot];
        
        Item originalItem = itemInSlots[originalSlot];
        int originalAmount = itemAmount[originalSlot];
        
        RemoveItemFromSlot(itemAmount[targetSlot], targetSlot);
        RemoveItemFromSlot(itemAmount[originalSlot], originalSlot);
        
        AddItemToSlot(targetAmount, targetItem, targetSlot);
        AddItemToSlot(originalAmount, originalItem, originalSlot);
        
        UpdateGui();
        
        
    }
    private void ReturnToOriginalSlot(RectTransform targetObject)
    {
        if (slots[originalParentIndex] != null)
        {
            targetObject.transform.SetParent(originalParent);
            targetObject.transform.localPosition = Vector3.zero;
        }
    }
    private void TestInventory()
    {
        if (Input.GetKeyDown(KeyCode.G))
        {
            AddItemsToInventoryM(1, testItem1);
        }
        if (Input.GetKey(KeyCode.F))
        {
            AddItemsToInventoryM(1, testItem2);
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            RemoveItemsFromInventoryM(1);
        }
        
        if (Input.GetKeyDown(KeyCode.C))
        {
            ClearInventoryM();
        }
    }

    private void InventoryManager()
    {
        TestInventory();
        if (Input.GetKey(KeyCode.E))
        {
            inventory.SetActive(!inventory.activeSelf);
        }
    }
    private void AddItemsToInventoryM(int itemAmount, Item item)
    {
        for(int i = 0; i < slots.Length; i++)
        {
            if (itemInSlots[i] == null || (itemInSlots[i] == item && this.itemAmount[i] < itemInSlots[i].maxStackSize))
            {
                int lastSpace;
                if (itemInSlots[i] != null)
                {
                    lastSpace = itemInSlots[i].maxStackSize - this.itemAmount[i];
                }
                else
                {
                    lastSpace = itemAmount;
                }
                if (itemAmount <= lastSpace)
                {                
                    AddItemToSlot(itemAmount, item, i);
                    return;
                }
                else
                {
                    AddItemToSlot(lastSpace, item, i);
                    itemAmount -= lastSpace;
                }

            }
        }
    }

    private void RemoveItemsFromInventoryM(int itemAmount)
    {
        int lastItemAmount = itemAmount;
        for (int i = 0; i < slots.Length; i++)
        {
            int lastMinAmount = Mathf.Min(lastItemAmount, this.itemAmount[i]);
            RemoveItemFromSlot(lastMinAmount, i);
            lastItemAmount -= lastMinAmount;
            if (lastItemAmount <= 0)
            {
                return;
            }
        }
    }
    private void RemoveItemsFromInventoryMD(int itemAmount, Item item)
    {
        int lastItemAmount = itemAmount;
        for (int i = 0; i < slots.Length; i++)
        {
            if (itemInSlots[i] == item)
            {
                RemoveItemFromSlot(Mathf.Min(lastItemAmount, this.itemAmount[i]), i);
                lastItemAmount -= Mathf.Min(lastItemAmount, this.itemAmount[i]);
                if (lastItemAmount <= 0)
                {
                    return;
                }
            }

        }
    }

    private void ClearInventoryM()
    {
        for (int i = 0; i < slots.Length; i++)
        {
            itemAmount[i] = 0;
            itemInSlots[i] = null;
            slotsImages[i].GetComponent<Image>().sprite = null;
            slotsTexts[i].GetComponent<Text>().text = "";
            UpdateGui();
        }
    }
    private void RemoveItemFromSlot(int amount, int slot)
    {
        itemAmount[slot] -= amount;
        if (itemAmount[slot] <= 0)
        {
            itemAmount[slot] = 0;
            itemInSlots[slot] = null;
            UpdateGui();
            
        }
        UpdateGui();

    }
    private void AddItemToSlot(int amount, Item item, int slot)
    {
        itemInSlots[slot] = item;
        itemAmount[slot] += amount;
        UpdateGui();
        
    }

    private void UpdateGui()
    {
        for (int i = 0; i < slots.Length; i++)
        {
            Image slotImage = slotsImages[i].GetComponent<Image>();
            if (slotImage.enabled == false &&itemAmount[i]>0)
            {
                slotsImages[i].GetComponent<Image>().enabled = true;
            }
            else if(itemAmount[i]<=0)
            {

                slotsImages[i].GetComponent<Image>().enabled = false;
            }
            if(itemInSlots[i] != null){ slotsImages[i].GetComponent<Image>().sprite = itemInSlots[i].icon;}
            else
            {
                if(slotsImages[i] != null) {slotsImages[i].GetComponent<Image>().sprite = null;}
                else
                {
                    Debug.Log($"slots Images is {slotsImages[i]}, index {i}");
                }
            }
            if(slotsTexts[i] != null){ slotsTexts[i].GetComponent<Text>().text = itemAmount[i].ToString();}
            else
            {
                Debug.Log($"slots Texts is {slotsTexts[i]}, index {i}");
            }
            if (itemAmount[i] <= 1)
            {
                if(slotsTexts[i] != null){ slotsTexts[i].GetComponent<Text>().text =  "";}
                else
                {
                    Debug.Log($"slots Texts is {slotsTexts[i]}, index {i}");
                }
            }
        }
    }
}
