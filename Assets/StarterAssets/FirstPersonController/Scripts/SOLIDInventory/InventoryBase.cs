using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Threading.Tasks;
using StarterAssets.FirstPersonController.Scripts.SOLIDInventory;
public abstract class InventoryBase : MonoBehaviour
{
    [Header("Inventory Settings")]
    [SerializeField] protected int maxSlots = 32;
    [SerializeField] protected bool allowStacking = true; 
    protected DragAndDrop dragAndDrop;
    [SerializeField] protected Canvas InventoryCanvas;
    protected List<InventorySlots> slots {get; private set; } = new List<InventorySlots>();
    protected InventoryParams parameters;
    protected HashSet<Image> imageArrayChecker;
    protected HashSet<GameObject> slotsArrayChecker;
    [SerializeField]protected GameObject inventory;
    [SerializeField]protected GameObject hotbar;
    [SerializeField]protected float dragspeed;
    protected virtual void Awake()=> InitializeInventory();
    protected virtual void Start()=>SingOnEvents();
    protected virtual void OnDestroy()=> SingOffEvents();
    protected abstract void InitializeInventory();
    protected abstract void SingOnEvents();
    protected abstract void SingOffEvents();
    public abstract Task<int> AddItem(Item item, int amount);
    public abstract Task<bool> RemoveItem(int amount);
    public abstract Task ClearInventory();
    
}