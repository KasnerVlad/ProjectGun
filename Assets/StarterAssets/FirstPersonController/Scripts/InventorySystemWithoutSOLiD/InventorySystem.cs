using System.Collections.Generic;
using StarterAssets;
using UnityEngine;
using UnityEngine.UI;
using Image = UnityEngine.UI.Image;
using Cursor = UnityEngine.Cursor;

public class InventorySystem : MonoBehaviour
{
    [Header("Inventory")]
    private List<InventorySlot> inventorySlots = new List<InventorySlot>(32);
    [SerializeField] private GameObject inventory;
    [SerializeField] private GameObject hotBar;
    private DradAndDrop _dradAndDrop = new DradAndDrop();

    private int currentSlot;

    [Header("Drag and Drop")]
    private HashSet<Image> imageArrayChecker;
    private HashSet<GameObject> slotsArrayChecker;
    public Canvas canvas;

    [Header("TestInventory")]
    [SerializeField] private Item testItem1;
    [SerializeField] private Item testItem2;
    
    [Header("Delegats")]
    
    public static DragAndDropHandler OnDragAndDrop;
    public static InventoryManagerHandler OnInventory;
    public static AddItemToInventoryHandler OnAddItemToInventory;
    public static RemoveItemFromInventoryHandler OnRemoveItemFromInventory;
    public delegate void DragAndDropHandler(float dragSpeed);
    public delegate void InventoryManagerHandler();
    public delegate bool AddItemToInventoryHandler(int amount,Item item);
    public delegate bool RemoveItemFromInventoryHandler(int amount);

    
    [Header("Others")]
    private FirstPersonController firstPersonController;
    [SerializeField] private Transform spawnPrefabPoint;
    [SerializeField] private GameObject player;

    public GameObject InstatePrefab(GameObject prefab) { GameObject g = Instantiate(prefab, spawnPrefabPoint.position, Quaternion.identity); return g; }
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
        ClearInventoryM();
        StartLogic();
        imageArrayChecker = new HashSet<Image>(GetSlotImage());
        slotsArrayChecker = new HashSet<GameObject>(GetInventorySlotsArray());
        UpdateGuiInArray();
        _dradAndDrop.SetImageArray(imageArrayChecker);
        _dradAndDrop.SetCanvas(canvas);
        _dradAndDrop.SetSlotArray(slotsArrayChecker);
        _dradAndDrop.SetInventory(inventory);
        _dradAndDrop.SetInventorySlots(inventorySlots);
        OnDragAndDrop = _dradAndDrop.DragAndDropManager;
        OnInventory = InventoryManager;
        OnAddItemToInventory = AddItemsToInventoryM;
        OnRemoveItemFromInventory = RemoveItemsFromInventoryM;

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
    private void UnActiveInventoryPanel() { inventory.SetActive(false); }
    // Update is called once per frame
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
    private bool AddItemsToInventoryM(int itemAmount, Item item)
    {
        int slotIndexWithItem=-1;
        for(int i = 0; i < inventorySlots.Count; i++)
        {
            for (int j = 0; j < inventorySlots.Count; j++)
            {
                if (inventorySlots[j].GetItem() == item && inventorySlots[j].GetAmount()<inventorySlots[j].GetItem().maxStackSize) { slotIndexWithItem = j; }
            }
            if (inventorySlots[i].GetItem() == null || (inventorySlots[i].GetItem() == item && inventorySlots[i].GetAmount() < inventorySlots[i].GetItem().maxStackSize))
            {
                int lastSpace;
                int index = slotIndexWithItem != -1 ? slotIndexWithItem : i;
                if (inventorySlots[index].GetItem() != null) {lastSpace = inventorySlots[index].GetItem().maxStackSize - inventorySlots[index].GetAmount(); }
                else { lastSpace = itemAmount; }
                if (itemAmount <= lastSpace)
                {                
                    inventorySlots[index].AddItem(item, itemAmount);
                    return true;
                }
                else
                {
                    inventorySlots[index].AddItem(item, lastSpace);
                    itemAmount -= lastSpace;
                }

            }
            

        }
        Debug.Log("No space to add item in inventory");
        return false;
    }

    private bool RemoveItemsFromInventoryM(int itemAmount)
    {
        int lastItemAmount = itemAmount;
        for (int i = 0; i < inventorySlots.Count; i++)
        {
            int lastMinAmount = Mathf.Min(lastItemAmount, inventorySlots[i].GetAmount());
            inventorySlots[i].RemoveItem(lastMinAmount);
            lastItemAmount -= lastMinAmount;
            if (lastItemAmount <= 0) return true;
        }
        Debug.Log("No items to remove in inventory");
        return false;
    }
    private void ClearInventoryM() { for (int i = 0; i < inventorySlots.Count; i++) { inventorySlots[i].ClearSlot(); } }

    private void UpdateGuiInArray() { for (int i = 0; i < inventorySlots.Count; i++) { inventorySlots[i].UpdateGui(); } }
}
