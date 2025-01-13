using System;
using System.Collections.Generic;
using StarterAssets;
using UnityEngine;
using UnityEngine.UI;
using Image = UnityEngine.UI.Image;
using UnityEngine.EventSystems;
using Cursor = UnityEngine.Cursor;

public class InventorySlot
{
    private GameObject slot;
    private Image slotImage;
    private Text slotText;
    private int itemAmount;
    private Item itemInSlot;
    public void ClearSlot()
    {
        itemAmount = 0;
        itemInSlot = null;
        slotImage.sprite = null;
        slotText.text = "";
        Debug.Log($"inventory slot cleared, amount: {itemAmount}, item: {itemInSlot}, slot Image: {slotImage.gameObject.name}, slot Text: {slotText.name}");
    }
    public bool AddItem(Item item, int amount)
    {
        if (itemAmount + amount <= item.maxStackSize)
        {
            itemInSlot = item;
            itemAmount += amount;
            UpdateGui(); return true;
        }
        else { return false; }
    }
    public bool RemoveItem(int amount)
    {
        itemAmount -= amount;
        if (itemAmount <= 0)
        {
            itemAmount = 0;
            itemInSlot = null;
            UpdateGui(); return false;
        }
        UpdateGui(); return true;
    }
    public void UpdateGui()
    {
        if (slotImage != null)
        {
            slotImage.sprite=itemInSlot==null?null:itemInSlot.icon;
            if (itemAmount <= 0) { slotImage.enabled = false; }
            else { slotImage.enabled = true; }
            
        }
        else { throw new Exception("Can't update image in slot"); }
        if (slotText != null)
        {
            slotText.text = itemAmount.ToString();
            if (itemAmount <= 1) { slotText.text = ""; }
        }
        else { throw new Exception("Can't update text in slot"); }
    }
    public void SetSlotAmount(int amount) { itemAmount = amount; UpdateGui(); }
    public int GetAmount() { return itemAmount; }
    public void SetSlotItem(Item item) { itemInSlot = item; UpdateGui(); }
    public Item GetItem() { return itemInSlot; }
    public void SetInventorySlot(GameObject slot) { this.slot = slot; }
    public GameObject GetInventorySlot() { return slot; }
    public void SetSlotImage(Image image) { slotImage = image; }
    public Image GetSlotImage() { return slotImage; }
    public void SetSlotText(Text text) { slotText = text; }
    public Text GetSlotText() { return slotText; }
}

public class InventorySystem : MonoBehaviour
{
    [Header("Inventory")]
    [SerializeField] private List<InventorySlot> inventorySlots = new List<InventorySlot>(32);
    [SerializeField] private GameObject inventory;
    [SerializeField] private GameObject hotBar;

    private int currentSlot;

    [Header("Drag and Drop")]
    public bool dragging;
    private RectTransform rectTransform;
    private Transform originalParent;
    private int originalParentIndex;
    private HashSet<Image> imageArrayChecker;
    private HashSet<GameObject> slotsArrayChecker;
    
    public Canvas canvas;
    private CanvasGroup canvasGroup;
    public float dragSpeed = 1f;


    
    [Header("TestInventory")]
    [SerializeField] private Item testItem1;
    [SerializeField] private Item testItem2;
    
    [Header("Others")]
    private FirstPersonController firstPersonController;
    [SerializeField] private Transform spawnPrefabPoint;
    [SerializeField] private GameObject player;
 // Объявление как поле класса

    private void ResetSlotsValues()
    {
        for (int i = 0; i < inventorySlots.Count; i++)
        {
            inventorySlots[i].ClearSlot();

        }

    }

    private void GetInventorySlots()
    {
        for (int i = 0; i < inventorySlots.Count; i++)
        {
            inventorySlots[i].SetInventorySlot(i<hotBar.transform.childCount ? 
                hotBar.transform.GetChild(i).gameObject :
                inventory.transform.GetChild(i-hotBar.transform.childCount).gameObject);

        }

    }

    private void GetInventoryOutputs()
    {
        for (int i = 0; i < inventorySlots.Count; i++)
        {
            inventorySlots[i].SetSlotImage(inventorySlots[i].GetInventorySlot().transform.GetChild(0).gameObject.GetComponent<Image>());
            inventorySlots[i].SetSlotText(inventorySlots[i].GetInventorySlot().transform.GetChild(0).gameObject.transform.GetChild(0).gameObject.GetComponent<Text>());

        }

    }

    private Image[] GetSlotImage()
    {
        Image[] imageArray = new Image[inventorySlots.Count];
        for (int i = 0; i < inventorySlots.Count; i++)
        {
            imageArray[i] = inventorySlots[i].GetSlotImage();
        }
        return imageArray;
    }

    private GameObject[] GetInventorySlotsArray()
    {
        GameObject[] slotsArray = new GameObject[inventorySlots.Count];
        for (int i = 0; i < inventorySlots.Count; i++)
        {
            slotsArray[i] = inventorySlots[i].GetInventorySlot();
        }
        return slotsArray;
    }
    void Start()
    {
        firstPersonController = player.GetComponent<FirstPersonController>();
        for (int i = 0; i < 32; i++) // Или inventorySlots.Capacity, если вы хотите использовать заданную вместимость
        {
            inventorySlots.Add(new InventorySlot()); // Создаем новый экземпляр слота
        }
        GetInventorySlots();
        GetInventoryOutputs();
        ResetSlotsValues();
        StartLogic();
        imageArrayChecker = new HashSet<Image>(GetSlotImage());
        slotsArrayChecker = new HashSet<GameObject>(GetInventorySlotsArray());
        UpdateGuiInArray();

    }

    private void StartLogic()
    {

        for (int i = 0; i < inventorySlots.Count; i++)
        {
            if (inventorySlots[i].GetSlotImage() != null)
            {
                inventorySlots[i].GetSlotImage().enabled = false;
            }
            else
            {
                Debug.Log($"Image: {GetInventorySlotsArray()[i].transform.GetChild(0).gameObject}, index: {i} is null");
            }
        }
        Invoke("UnActiveInventoryPanel", 0.01f);
    }
    private void UnActiveInventoryPanel()
    {
        inventory.SetActive(false);
    }
    // Update is called once per frame
    void Update()
    {
        DragAndDropManager();
        InventoryManager();
    }

    private void DragAndDropManager()
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
            else
            {
                Debug.Log("Results.Count = 0");
            }
        }

        if (Input.GetMouseButton(0) && dragging&&inventory.activeSelf)
        {

            Vector2 delta = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));

            rectTransform.anchoredPosition += delta * dragSpeed;
        }

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
                Instantiate(inventorySlots[originalParentIndex].GetItem().prefab, spawnPrefabPoint.position, spawnPrefabPoint.rotation);
                inventorySlots[originalParentIndex].RemoveItem(inventorySlots[originalParentIndex].GetAmount());
            }
            else if(results.Count <= 1)
            {
                Debug.Log($"Prefab in {inventorySlots[originalParentIndex].GetItem()} not found");
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
                            ReturnToOriginalSlot(rectTransform);
                            return;
                            
                        }
                        if (inventorySlots[slotss.IndexOf(g)].GetItem() == inventorySlots[originalParentIndex].GetItem())
                        {
                            if (inventorySlots[slotss.IndexOf(g)].GetAmount() == inventorySlots[slotss.IndexOf(g)].GetItem().maxStackSize)
                            {
                                SwapItems(originalParentIndex, slotss.IndexOf(g));
                                Debug.Log("Swapped Item");
                                ReturnToOriginalSlot(rectTransform);
                                return;
                            }
                            int remainingSpace = inventorySlots[slotss.IndexOf(g)].GetItem().maxStackSize - inventorySlots[slotss.IndexOf(g)].GetAmount();
                            int amountToMove = Mathf.Min(inventorySlots[originalParentIndex].GetAmount(), remainingSpace);
                            int originalSlotNewAmount = inventorySlots[originalParentIndex].GetAmount() - amountToMove;
                            MoveItems(slotss.IndexOf(g), amountToMove, originalSlotNewAmount, originalParentIndex);
                            ReturnToOriginalSlot(rectTransform);
                            return;
                        }
                        else if (inventorySlots[slotss.IndexOf(g)].GetItem() == null)
                        {
                            MoveAllItems(slotss.IndexOf(g), originalParentIndex);
                            ReturnToOriginalSlot(rectTransform);
                            return;
                        }
                        else if (inventorySlots[slotss.IndexOf(g)].GetItem()!= null &&
                                 inventorySlots[slotss.IndexOf(g)].GetItem() != inventorySlots[originalParentIndex].GetItem())
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
        Item originalItem = inventorySlots[originalSlot].GetItem();
        int originalAmount = inventorySlots[originalSlot].GetAmount();

        inventorySlots[targetSlot].AddItem(originalItem, originalAmount);
        inventorySlots[originalSlot].RemoveItem(originalAmount);
        UpdateGuiInArray();
    }
    private void MoveItems(int targetSlot, int amountToMove, int originalSlotNewAmount, int originalSlot)
    {

        Item originalItem = inventorySlots[originalSlot].GetItem();
        Item targetItem = inventorySlots[targetSlot].GetItem();
        int originalAmount = inventorySlots[originalSlot].GetAmount();
        inventorySlots[targetSlot].AddItem(targetItem, amountToMove);
        inventorySlots[originalSlot].RemoveItem(originalAmount);
        inventorySlots[originalSlot].AddItem(originalItem, originalSlotNewAmount);
        UpdateGuiInArray();
    }
    private void SwapItems(int originalSlot, int targetSlot)
    {
        Item targetItem = inventorySlots[targetSlot].GetItem();
        int targetAmount = inventorySlots[targetSlot].GetAmount();
        
        Item originalItem = inventorySlots[originalSlot].GetItem();
        int originalAmount = inventorySlots[originalSlot].GetAmount();
        
        inventorySlots[targetSlot].RemoveItem(inventorySlots[targetSlot].GetAmount());
        inventorySlots[originalSlot].RemoveItem(inventorySlots[originalSlot].GetAmount());
        
        inventorySlots[originalSlot].AddItem(targetItem, targetAmount);
        inventorySlots[targetSlot].AddItem(originalItem, originalAmount);
        
        UpdateGuiInArray();
        
        
    }
    private void ReturnToOriginalSlot(RectTransform targetObject)
    {
        if (inventorySlots[originalParentIndex].GetInventorySlot() != null)
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
        if (Input.GetKeyDown(KeyCode.E))
        {
            inventory.SetActive(!inventory.activeSelf);
            firstPersonController._input.cursorLocked = !inventory.activeSelf;
            firstPersonController._input.cursorInputForLook = !inventory.activeSelf;
            firstPersonController._input.look = Vector2.zero;
            Cursor.visible = inventory.activeSelf;
            Cursor.lockState = inventory.activeSelf? CursorLockMode.None : CursorLockMode.Locked;
        }
        
        
    }
    private void AddItemsToInventoryM(int itemAmount, Item item)
    {
        int slotIndexWithItem=-1;

        for(int i = 0; i < inventorySlots.Count; i++)
        {
            for (int j = 0; j < inventorySlots.Count; j++)
            {
                if (inventorySlots[j].GetItem() == item && inventorySlots[j].GetAmount()<inventorySlots[j].GetItem().maxStackSize)
                {
                    slotIndexWithItem = j;
                }
            }
            if (slotIndexWithItem!=-1)
            {
                int lastSpace;
                if (inventorySlots[slotIndexWithItem].GetItem() != null)
                {
                    lastSpace = inventorySlots[slotIndexWithItem].GetItem().maxStackSize - inventorySlots[slotIndexWithItem].GetAmount();
                }
                else
                {
                    lastSpace = itemAmount;
                }
                if (itemAmount <= lastSpace)
                {                
                    inventorySlots[slotIndexWithItem].AddItem(item, itemAmount);
                    return;
                }
                else
                {

                    inventorySlots[slotIndexWithItem].AddItem(item, lastSpace);
                    itemAmount -= lastSpace;
                }
            }
            else
            {
                if (inventorySlots[i].GetItem() == null || (inventorySlots[i].GetItem() == item && inventorySlots[i].GetAmount() < inventorySlots[i].GetItem().maxStackSize))
                {

                    int lastSpace;
                    if (inventorySlots[i].GetItem() != null)
                    {
                        lastSpace = inventorySlots[i].GetItem().maxStackSize - inventorySlots[i].GetAmount();
                    }
                    else
                    {
                        lastSpace = itemAmount;
                    }
                    if (itemAmount <= lastSpace)
                    {                
                        inventorySlots[i].AddItem(item, itemAmount);
                        return;
                    }
                    else
                    {
                        inventorySlots[i].AddItem(item, lastSpace);
                        itemAmount -= lastSpace;
                    }

                }
            }

        }
    }

    private void RemoveItemsFromInventoryM(int itemAmount)
    {
        int lastItemAmount = itemAmount;
        for (int i = 0; i < inventorySlots.Count; i++)
        {
            int lastMinAmount = Mathf.Min(lastItemAmount, inventorySlots[i].GetAmount());
            inventorySlots[i].RemoveItem(lastMinAmount);
            lastItemAmount -= lastMinAmount;
            if (lastItemAmount <= 0)
            {
                return;
            }
        }
    }
    private void ClearInventoryM()
    {
        for (int i = 0; i < inventorySlots.Count; i++)
        {
            inventorySlots[i].ClearSlot();
        }
    }

    private void UpdateGuiInArray()
    {
        for (int i = 0; i < inventorySlots.Count; i++)
        {
            inventorySlots[i].UpdateGui();
        }
    }
}
