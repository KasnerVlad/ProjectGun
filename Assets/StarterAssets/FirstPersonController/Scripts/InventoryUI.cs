using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Threading.Tasks;
public class InventoryUI : MonoBehaviour
{
    [SerializeField] private List<InventorySlots> slots;
    private List<Text> slotTexts = new List<Text>();
    private List<Image> slotImages = new List<Image>();
    
    private void Start()
    {
        InitializeSlots();
        InventoryEvents.OnInventoryUpdated += UpdateUI;

    }

    private void InitializeSlots()
    {
        DragAndDrop dragAndDrop = GetComponent<DragAndDrop>();
        slots = new List<InventorySlots>(dragAndDrop.inventorySlots);
        for (int i = 0; i < 32; i++)
        {
            slotTexts.Add(slots[i].Slot.transform.GetChild(0).GetChild(0).GetComponent<Text>());
            slotImages.Add(slots[i].Slot.transform.GetChild(0).GetComponent<Image>());
        }
    }
    private void OnDestroy()
    {
        InventoryEvents.OnInventoryUpdated -= UpdateUI;
    }
    
    private async Task UpdateUI(List<InventorySlots> slots)
    {
        await Task.Yield();
        Debug.Log("Inventory UI Updated");
        for (int i = 0; i < slots.Count; i++)
        {
            slotTexts[i].text = slots[i].Amount<=1?"" : slots[i].Amount.ToString();
            slotImages[i].sprite =slots[i].Amount<=0? null : slots[i].Item.icon;
            slotImages[i].enabled = !(slots[i].Amount<=0);
        }
    }
}