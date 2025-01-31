using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class InventoryBase : MonoBehaviour
{
    [Header("Inventory Settings")]
    [SerializeField] protected int maxSlots = 32;
    [SerializeField] protected bool allowStacking = true; 
    protected DragAndDrop dragAndDrop;
    [SerializeField] protected Canvas InventoryCanvas;
    protected List<InventorySlots> slots;
    protected InventoryParams parameters;
    protected HashSet<Image> imageArrayChecker;
    protected HashSet<GameObject> slotsArrayChecker;
    [SerializeField]protected GameObject inventory;
    [SerializeField]protected GameObject hotbar;
    [SerializeField]protected float dragspeed;
    protected virtual void Awake()
    {
        InitializeInventory();
    }

    protected virtual void Start()
    {
        SingOnEvents();
        
    }
    
    protected virtual void OnDestroy()
    {
        SingOffEvents();
    }
    public abstract void InitializeInventory();
    public abstract void SingOnEvents();
    public abstract void SingOffEvents();
    public abstract bool AddItem(Item item, int amount);
    public abstract bool RemoveItem(int amount);
    public abstract void ClearInventory();
    
}